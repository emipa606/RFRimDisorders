# GitHub Copilot Instructions for [RF] Rim Disorders (Continued) Mod

## Mod Overview and Purpose

[RF] Rim Disorders (Continued) is a RimWorld mod that adds more mental health disorders and counseling system to your colony. The mod was originally created by SeveralPuffins and updated by Rainbeau Flambe and firefoxpdm.

## Key Features and Systems

- **Illnesses**: Adds several mental illnesses including Depression, Generalized Anxiety, Cleanliness-OCD, and PTSD. These illnesses can be triggered by random chance, extreme low mood, traumatic events, and other factors.
- **Disorders**: Adds Autism Spectrum Disorder and ADHD. These disorders affect social interactions and work behavior. ADHD-triggered recreation interruptions are autonomous and must not be treated as player-prioritized work by Autism effects.
- **Counseling**: Adds a counseling system that can help reduce the severity of illnesses and disorders. A counselor must be trained in Medicine and have high Social skill to perform the bill.

## Coding Patterns and Conventions

- **C# Files**: The mod uses C# to create custom classes and functions to handle various aspects of the mod. The following are some of the key C# files in the mod:
  - `Alert_SudicideRisk.cs`: Defines an alert for suicide risk.
  - `Hediff_PTSD.cs`: Defines the PTSD hediff and its behavior.
  - `Controller.cs`: Handles settings and options for the mod.
  - `DiseaseDefOfRimDisorders.cs`: Defines disease constants.
  - `Hediff_ADHD.cs`: Defines the ADHD hediff and its behavior.
- **XML Files**: The mod uses XML to define various aspects of the mod, including hediffs, mental breaks, recipes, and thoughts. The following are some of the key XML files in the mod:
  - `Hediffs_MentalIllnesses.xml`: Defines the mental illness hediffs.
  - `MentalBreaks.xml`: Defines the mental break effects.
  - `RecipesCounseling.xml`: Defines the counseling recipes.
  - `Thoughts_Illnesses.xml`: Defines the thoughts related to illnesses.

## XML Integration

The mod uses XML to define various aspects of the mod, including hediffs, mental breaks, recipes, and thoughts. The XML files are located in the `Defs` folder of the mod.

- **Hediffs_MentalIllnesses.xml**: Defines the mental illness hediffs.
- **MentalBreaks.xml**: Defines the mental break effects.
- **RecipesCounseling.xml**: Defines the counseling recipes.
- **Thoughts_Illnesses.xml**: Defines the thoughts related to illnesses.

To integrate XML into the mod, you can use the `DefOf` pattern to define constants for your hediffs, recipes, and thoughts. For example, you can define a constant for a new hediff like this:

xml
<HediffDef>
  <defName>MyNewHediff</defName>
  <label>My New Hediff</label>
  <description>This is my new hediff.</description>
</HediffDef>


Then, you can use this constant in your C# code like this:

csharp
public static HediffDef MyNewHediff = DefDatabase<HediffDef>.GetNamed("MyNewHediff");


## Harmony Patching

The mod uses Harmony patching to modify existing RimWorld code. Harmony patching allows you to modify the behavior of existing methods without changing the original code.

To use Harmony patching, you need to create a patch class that inherits from `HarmonyPatch`. For example, you can create a patch class for a method like this:

csharp
[HarmonyPatch(typeof(SomeClass), "SomeMethod")]
public static class SomePatch
{
  [HarmonyPostfix]
  public static void Postfix(SomeClass __instance)
  {
    // your patch code here
  }
}


In this example, the `Postfix` method is called after the original method. You can also create a `Prefix` method that is called before the original method.

To apply the patch, you need to call the `Patch` method in the `HarmonyInstance` class. For example:

csharp
HarmonyInstance harmony = HarmonyInstance.Create("com.mycompany.mygame");
harmony.PatchAll(Assembly.GetExecutingAssembly());


## Suggestions for Copilot

- **Code Completion**: Use Copilot to complete your C# code, including class definitions, method implementations, and other code snippets.
- **XML Snippets**: Use Copilot to generate XML snippets for your hediffs, recipes, and thoughts.
- **Harmony Patches**: Use Copilot to generate Harmony patch classes and methods.

By following these instructions, you can effectively use GitHub Copilot to develop and maintain your RimWorld mod.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

