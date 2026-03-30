using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Winslopr.Helpers
{
    public static class Localizer
    {
        private static ResourceLoader? _loader;

        private static ResourceLoader Loader => _loader ??= new ResourceLoader();

        /// <summary>
        /// Discovers available languages by scanning Strings/ subfolders
        /// next to the exe. Returns BCP-47 tags (e.g. "en-US", "de-DE").
        /// </summary>
        public static IReadOnlyList<string> GetAvailableLanguages()
        {
            var languages = new List<string>();
            string stringsDir = Path.Combine(AppContext.BaseDirectory, "Strings");
            if (!Directory.Exists(stringsDir)) return languages;

            foreach (var dir in Directory.GetDirectories(stringsDir))
            {
                string tag = Path.GetFileName(dir);
                if (File.Exists(Path.Combine(dir, "Resources.resw")))
                    languages.Add(tag);
            }
            return languages;
        }

        /// <summary>
        /// Returns the active language code (e.g. "en-US", "de-DE").
        /// </summary>
        public static string CurrentLanguage
            => SettingsHelper.GetLanguage()
            ?? System.Globalization.CultureInfo.CurrentUICulture.Name
            ?? "en-US";

        // Saves the selected language, reloads the resource loader, and shows
        // a restart prompt. If the .resw file contains translator credits
        // (Language_TranslatorName / Language_TranslatorWebsite), they are
        // appended to the dialog so translators get visible attribution.
        public static async Task SwitchLanguageAsync(string langCode, XamlRoot xamlRoot)
        {
            SettingsHelper.SetLanguage(langCode);
            Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = langCode;
            _loader = null;

            // Translator credits are shown in Settings > Language
            string message = Get("Language_RestartRequired");

            await new ContentDialog
            {
                Title = Get("Language_RestartTitle"),
                Content = message,
                CloseButtonText = Get("Common_OK"),
                XamlRoot = xamlRoot,
                RequestedTheme = App.CurrentTheme
            }.ShowAsync();
        }

        // Gets a localized string by key
        public static string Get(string key)
        {
            try
            {
                string? value = Loader.GetString(key);
                return string.IsNullOrWhiteSpace(value) ? key : value;
            }
            catch
            {
                return key;
            }
        }

        // Gets a formatted localized string
        public static string GetFormat(string key, params object[] args)
        {
            string template = Get(key);
            try
            {
                return string.Format(template, args);
            }
            catch
            {
                return template;
            }
        }
    }
}
