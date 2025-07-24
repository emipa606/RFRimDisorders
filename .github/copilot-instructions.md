# GitHub Copilot Instructions for RimWorld Modding Project

Welcome to the RimWorld modding project, which extends the vanilla game by adding a series of mental illness mechanics designed to enhance gameplay with new challenges and opportunities for storytelling.

## Mod Overview and Purpose

This mod aims to bring realistic and impactful mental health conditions into the RimWorld universe, allowing players to manage and treat various mental health issues among their colonists. By introducing mental illnesses like ADHD, Autism, Generalized Anxiety, and more, the mod adds depth to the psychological and social aspects of the game.

## Key Features and Systems

- **Mental Illness Mechanics**: Includes a variety of mental illnesses each with unique effects and behaviors, represented through classes that extend the `MentalIllness` base class.
- **Counseling System**: Characters can engage in counseling sessions to manage or alleviate the effects of mental illnesses.
- **Mental States and Alerts**: Features critical alerts for suicide risks and special mental states like panic attacks, enhancing the game's realism and urgency.
- **Dynamic Events**: Random events can trigger new mental illnesses, adding unpredictability and challenges to colony management.
- **Thought Workers**: Implements specific thought patterns and behaviors influenced by the mental conditions of colonists.

## Coding Patterns and Conventions

- **Class Structure**: Each mental illness is encapsulated within its own class, inheriting from the `MentalIllness` abstract class, ensuring modularity and ease of management.
- **Conventions**: Following standard C# coding conventions with PascalCase for class names and methods. Private methods are prefixed with a lowercase initial letter.
- **Component-Based Design**: Uses `HediffComp` to attach additional properties and logic to instances of mental illnesses.

## XML Integration

- **DiseaseDefs**: Utilize XML files to define disease properties and integrate them with the game's existing mechanics.
- **ThoughtDefs and IncidentDefs**: XML files are used to describe thought patterns and random incidents related to mental illnesses, which the codebase then references.
- Considerate use of XML allows for easy updates and modifications without altering the core logic in C#.

## Harmony Patching

- **Purpose**: The mod utilizes Harmony to patch existing RimWorld methods. This allows the introduction of new behaviors without directly modifying the game's source code.
- **Usage**: Apply patches to methods where changes or new mechanics need to be introduced, ensuring the mod remains compatible with other mods and game updates.
- **Considerations**: Always ensure that patches are safe and avoid conflicts with the vanilla game and other mods.

## Suggestions for Copilot

- **Pattern Recognition**: Encourage Copilot to identify and suggest code snippets that conform to established C# patterns and conventions used throughout the mod, such as class inheritance hierarchies.
- **XML and C# Integration**: Guide Copilot to facilitate the integration between XML definitions and C# classes. For instance, it could suggest how to bind XML disease definitions with C# disease-effect logic.
- **Modular Enhancements**: Leverage Copilot to suggest modular enhancements or refactorings that could simplify the addition of new illnesses or events to the system.
- **Error Handling and Optimization**: Use Copilot to propose error handling strategies and code optimizations to ensure smooth mod operations in various in-game scenarios.

By following these guidelines, you can make the most out of Copilot in maintaining a high standard of coding quality and mod functionality in your RimWorld modding project.
