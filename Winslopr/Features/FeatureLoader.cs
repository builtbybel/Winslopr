using Settings.Ads;
using Settings.AI;
using Settings.Edge;
using Settings.Gaming;
using Settings.Issues;
using Settings.Personalization;
using Settings.Privacy;
using Settings.System;
using Settings.Taskbar;
using Settings.UI;
using System.Collections.Generic;
using Winslop.Helpers;

namespace Winslop.Features
{
    public static class FeatureLoader
    {
        public static List<FeatureTreeItem> Load()
        {
            return new List<FeatureTreeItem>
            {
               new FeatureTreeItem(Localizer.Get("Category_Issues"))
                {
                    Children =
                    {
                        new FeatureTreeItem(new BasicCleanup()),
                    }
                },

              new FeatureTreeItem(Localizer.Get("Category_System"))
                {
                    Children =
                    {
                       new FeatureTreeItem(new BSODDetails()),
                       new FeatureTreeItem(new VerboseStatus()),
                       new FeatureTreeItem(new SpeedUpShutdown()),
                       new FeatureTreeItem(new NetworkThrottling()),
                       new FeatureTreeItem(new SystemResponsiveness()),
                       new FeatureTreeItem(new MenuShowDelay()),
                       new FeatureTreeItem(new DisableHibernation()),
                    }
                },

                new FeatureTreeItem(Localizer.Get("Category_MSEdge"))
                {
                    Children =
                    {
                       new FeatureTreeItem(new BrowserSignin()),
                       new FeatureTreeItem(new DefaultTopSites()),
                       new FeatureTreeItem(new DefautBrowserSetting()),
                       new FeatureTreeItem(new EdgeCollections()),
                       new FeatureTreeItem(new EdgeShoppingAssistant()),
                       new FeatureTreeItem(new FirstRunExperience()),
                       new FeatureTreeItem(new GamerMode()),
                       new FeatureTreeItem(new HubsSidebar()),
                       new FeatureTreeItem(new ImportOnEachLaunch()),
                       new FeatureTreeItem(new StartupBoost()),
                       new FeatureTreeItem(new TabPageQuickLinks()),
                       new FeatureTreeItem(new UserFeedback()),
                    }
                },

               new FeatureTreeItem(Localizer.Get("Category_UI"))
                {
                    Children =
                    {
                       new FeatureTreeItem(new FullContextMenus()),
                       new FeatureTreeItem(new LockScreen()),
                       new FeatureTreeItem(new ShowOrHideMostUsedApps()),
                       new FeatureTreeItem(new DisableBingSearch()),
                       new FeatureTreeItem(new StartLayout()),
                       new FeatureTreeItem(new Transparency()),
                       new FeatureTreeItem(new AppDarkMode(), defaultChecked: false),
                       new FeatureTreeItem(new SystemDarkMode(), defaultChecked: false),
                       new FeatureTreeItem(new DisableSnapAssistFlyout()),
                    }
                },

               new FeatureTreeItem(Localizer.Get("Category_Taskbar"))
                {
                    Children =
                    {
                       new FeatureTreeItem(new AlwaysShowTrayIcons()),
                       new FeatureTreeItem(new RemoveMeetNowButton()),
                       new FeatureTreeItem(new DisableNewsAndInterests()),
                       new FeatureTreeItem(new DisableWidgets()),
                       new FeatureTreeItem(new TaskbarEndTask()),
                       new FeatureTreeItem(new TaskbarSmallIcons()),
                       new FeatureTreeItem(new SearchboxTaskbarMode()),
                       new FeatureTreeItem(new ShowTaskViewButton()),
                       new FeatureTreeItem(new TaskbarAlignment()),
                       new FeatureTreeItem(new CleanTaskbar(), defaultChecked: false),
                    }
                },

               new FeatureTreeItem(Localizer.Get("Category_Gaming"))
                {
                    Children =
                    {
                       new FeatureTreeItem(new GameDVR()),
                       new FeatureTreeItem(new PowerThrottling()),
                       new FeatureTreeItem(new VisualFX()),
                    }
                },

                new FeatureTreeItem(Localizer.Get("Category_Privacy"))
                {
                    Children =
                    {
                       new FeatureTreeItem(new ActivityHistory()),
                       new FeatureTreeItem(new LocationTracking()),
                       new FeatureTreeItem(new PrivacyExperience()),
                       new FeatureTreeItem(new DiagnosticData()),
                       new FeatureTreeItem(new SilentAppInstallation()),
                       new FeatureTreeItem(new WindowsSpotlightLockScreen()),
                       new FeatureTreeItem(new LockScreenSlideshow()),
                       new FeatureTreeItem(new AppLaunchTracking()),
                       new FeatureTreeItem(new OnlineSpeechRecognition()),
                       new FeatureTreeItem(new NarratorOnlineServices()),
                    }
                },

                new FeatureTreeItem(Localizer.Get("Category_Ads"))
                {
                    Children =
                    {
                        new FeatureTreeItem(new FileExplorerAds()),
                        new FeatureTreeItem(new FinishSetupAds()),
                        new FeatureTreeItem(new LockScreenAds()),
                        new FeatureTreeItem(new PersonalizedAds()),
                        new FeatureTreeItem(new SettingsAds()),
                        new FeatureTreeItem(new StartmenuAds()),
                        new FeatureTreeItem(new TailoredExperiences()),
                        new FeatureTreeItem(new TipsAndSuggestions()),
                        new FeatureTreeItem(new WelcomeExperienceAds()),
                    }
                },

                new FeatureTreeItem(Localizer.Get("Category_AI"))
                {
                    Children =
                    {
                       new FeatureTreeItem(new CopilotTaskbar()),
                       new FeatureTreeItem(new Recall()),
                       new FeatureTreeItem(new ClickToDo(), defaultChecked: false),
                       new FeatureTreeItem(new DisableSearchBoxSuggestions()),
                    }
                },
            };
        }
    }
}