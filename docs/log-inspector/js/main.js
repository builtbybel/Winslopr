// Paste from clipboard and auto-analyze
async function pasteResult() {
  try {
    const text = await navigator.clipboard.readText();
    if (!text) return alert("Clipboard is empty.");
    document.getElementById("logInput").value = text;
    analyzeLog();
  } catch (err) {
    alert("Clipboard access denied: " + err);
  }
}

// Analyze log and render results with annoyance level
function analyzeLog() {
  const log = document.getElementById("logInput").value;
  const output = document.getElementById("output");
  const lines = log.split("\n");

  // Extract data
  const issues = lines.filter(l => l.startsWith("❌"));
  const regKeys = lines.filter(l => l.includes("HKEY_") || l.includes("➤"));
  const plugins = lines.filter(l => /Plugin ready:.*\.ps1/i.test(l));

  // Count total features: lines with status emoji (✅, ❌, ✔, ⚠)
  const featureLines = lines.filter(l => /^[✅❌✔⚠🔧↩️]/.test(l.trim()));
  const total = featureLines.length || 1;
  const annoyancePct = Math.round((issues.length / total) * 100);

  // Color for annoyance bar
  let barColor;
  if (annoyancePct === 0) barColor = "var(--green)";
  else if (annoyancePct < 20) barColor = "var(--yellow)";
  else barColor = "var(--red)";

  output.innerHTML =
    // Annoyance level bar
    `<div class="annoyance">
      <span class="annoyance-label">Annoyance Level</span>
      <div class="annoyance-bar">
        <div class="annoyance-fill" style="width:${annoyancePct}%;background:${barColor}"></div>
      </div>
      <span class="annoyance-pct" style="color:${barColor}">${annoyancePct}%</span>
    </div>` +

    // Issues
    `<h3>Found ${issues.length} issue(s)</h3>` +
    (issues.length
      ? issues.map(i => `<div class="item issue">${esc(i)}</div>`).join("")
      : `<div class="item">None — your Windows looks clean!</div>`) +

    `<hr>` +

    // Registry keys
    `<h3>Registry Keys</h3>` +
    (regKeys.length
      ? regKeys.map(k => `<div class="item key">${esc(k)}</div>`).join("")
      : `<div class="item">No registry keys found.</div>`) +

    `<hr>` +

    // Plugins
    `<h3>Loaded Plugins</h3>` +
    (plugins.length
      ? plugins.map(p => `<div class="item plugin">${esc(p)}</div>`).join("")
      : `<div class="item">No plugins detected.</div>`);
}

// HTML-escape to prevent XSS
function esc(str) {
  const d = document.createElement("div");
  d.textContent = str;
  return d.innerHTML;
}

// Screenshot the output section
function captureResult() {
  const el = document.getElementById("output");
  if (!el.innerText.trim()) return alert("No results to capture.");
  html2canvas(el).then(canvas => {
    const a = document.createElement("a");
    a.download = "Winslop-results.png";
    a.href = canvas.toDataURL();
    a.click();
  });
}

// Native share API
function shareResult() {
  const text = document.getElementById("output").innerText;
  if (!text.trim()) return alert("No results to share.");
  if (navigator.share) {
    navigator.share({ title: "Winslop Analysis", text }).catch(() => {});
  } else {
    alert("Sharing is not supported by your browser.");
  }
}

// Share on X/Twitter via tweet intent
function shareOnX() {
  const lines = document.getElementById("logInput").value.split("\n");
  const issues = lines.filter(l => l.startsWith("❌")).length;
  const featureLines = lines.filter(l => /^[✅❌✔⚠🔧↩️]/.test(l.trim()));
  const total = featureLines.length || 1;
  const pct = Math.round((issues / total) * 100);

  const text = issues > 0
    ? `My Windows has ${issues} annoyance(s) — Annoyance Level: ${pct}%! Scanned with #Winslop`
    : `My Windows is clean — 0% Annoyance Level! Checked with #Winslop`;

  const url = "https://github.com/builtbybel/Winslop";
  window.open(
    `https://twitter.com/intent/tweet?text=${encodeURIComponent(text)}&url=${encodeURIComponent(url)}`,
    "_blank"
  );
}

// Open Google Translate with log text
function translateViaGoogle() {
  const text = document.getElementById("logInput").value;
  if (!text.trim()) return alert("Paste a log first.");
  navigator.clipboard.writeText(text).catch(() => {});
  alert("Log copied to clipboard. Paste it into Google Translate.");
  window.open("https://translate.google.com/?sl=auto&tl=en&op=translate", "_blank");
}
