> **Note:**  
> "winslop" might refer to Windows 11 in some corners of the internet.  
> **Winslop**, however, is an actual tool designed to clean it up.
> 
# Built to remove things. Not to impress you.

> *“We need to get beyond the arguments of slop vs sophistication…”*  
> — Satya Nadella
> 
> “Slop” = stuff that ships by default, wastes resources, and takes control away, while doing basically nothing for you

This started as a joke.
Then Microsoft said “slop vs sophistication” out loud and it stopped being one.
If this tool needs marketing, it probably shouldnt exist 😆


To be clear:
**Windows isn’t bad.** I actually like it.
**But the direction of Windows 11 is.**  
So I built this again. Cheaper than therapy. Comes with toggles.

## What it does

Winslop does exactly what the name suggests:
- removes slop
- tones down what can’t be removed
- shows what’s running / enabled
- gives you control back

No cloud. No assistants. No “smart” magic.
Just local changes, explicit choices, and a way back.

Everything is deterministic and reversible.

## Where it comes from

**Under the hood this is a very small, very focused fork of my earlier project [CrapFixer](https://github.com/builtbybel/CrapFixer)**.
Localization never really happened there.
Winslop will likely get it soon, mostly because the codebase is drastically smaller.
Nothing fancy. Just tools.

## FAQ
<sub>Click to expand. No assistant required.</sub>

<details>
<summary><strong>Is Winslop anti-Windows?</strong></summary>

No.  
Windows itself is fine.  
The unnecessary stuff layered on top of it isn’t.

Winslop targets **slop**, not Windows.
</details>

<details>
<summary><strong>Is there a help file / quick start guide?</strong></summary>

Yes.  
There's a small, lightweight entry guide to get you going:

- **Quick Start / Help:** https://github.com/builtbybel/Winslop/blob/main/docs/Help.md
</details>

<details>
<summary><strong>Is this an anti-AI project?</strong></summary>

Also no.

Winslop doesn’t care whether something is AI, scripted, cloud-based or hand-written.  
If it’s forced, opaque, or unnecessary, its slop.

If it’s optional, transparent and useful, it’s fine.
</details>


<details>
<summary><strong>Does Winslop use AI?</strong></summary>

No.

There is no model, no prompt, no inference step.  
Everything is deterministic and based on explicit user choices.
</details>

<details>
<summary><strong>Is anything uploaded to the cloud?</strong></summary>

No.

Winslop does not upload logs, telemetry, system data or usage statistics.  
All actions happen locally on your machine.
</details>

<details>
<summary><strong>Can Winslop break my system?</strong></summary>

Any system tool can if misused.

That’s why Winslop:
- shows exactly what it will change  
- never runs anything automatically  
- allows changes to be reviewed and reverted  

You stay in control.
</details>


<details>
<summary><strong>Why not just use Copilot / Settings / PowerShell?</strong></summary>

You can.

Winslop exists for people who prefer:
- fewer layers  
- less abstraction  
- no opinions  
- no assistants  

Just tools.
</details>

<details>
<summary><strong>Is this related to CrapFixer?</strong></summary>

Yes.

Winslop is a very small, focused fork of CrapFixer.  
The goal was to remove complexity, reduce scope and make things easier to maintain.

Smaller codebase.  
Clearer intent.  
Less slop.
</details>

<details>
<summary><strong>Why the name?</strong></summary>

Because Microsoft said *“slop vs sophistication”* out loud.

And because Windows isn't bad.  
The slop is.

Winslop is **anti-slop by design**.

That’s why the UI is intentionally small, plain and functional.
No glossy visuals.  
No animations.  
No AI-generated fluff.

Slop comes in many forms:
- **AI slop** > mass-produced, soulless output
- **Feature slop** > unnecessary features that make UX worse
- **UX / Design slop** > bloated, unclear interfaces
- **Corporate slop** > buzzwords, filler and dark patterns

Winslop does the opposite:
It removes more than it adds.
</details>



<details>
<summary><strong>Confession: I like Windows, but...</strong></summary>

I just hate what Windows 11 keeps trying to become.  
So I built the “stop doing that” button again.  
Consider this stress relief with a UI.
</details>

<details>
<summary><strong>What is Winslop written in?</strong></summary>
Started in C, then I forked CrapFixer and finished it in C# and continued from there, just much smaller and more focused.
no glossy modern UI stack. No WinUI. No Mica. No overdesigned "modern Windows app" look. No "look how fancy we are" .
Just tools.
</details>


