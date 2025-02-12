# Mo.ProjectFilesToJson

A .NET console application designed to **scan the entire file structure** of a project (e.g., .NET, Java, Node, Python, Vue, or any other easily configurable type) and produce a consolidated **JSON** or **plain-text** output of those files. It is particularly useful for sharing large codebases with LLMs (Large Language Models such as ChatGPT) that have strict token or size limits.

---

## Table of Contents
1. [Why This Project?](#why-this-project)
2. [Project Highlights](#project-highlights)
3. [How It Works](#how-it-works)
4. [Installation & Setup](#installation--setup)
5. [Usage](#usage)
   - [Running the Console App](#running-the-console-app)
   - [Selecting Projects and Paths](#selecting-projects-and-paths)
   - [Formats: JSON or Simple Divider](#formats-json-or-simple-divider)
6. [Configuration](#configuration)
   - [Adding More Projects](#adding-more-projects)
   - [Handling .gitignore Files](#handling-gitignore-files)
   - [Custom Include/Exclude Patterns](#custom-includeexclude-patterns)
7. [Architecture Overview](#architecture-overview)
   - [Mo.ProjectFilesToJson.Console (Console Layer)](#moprojectfilestojsonconsole-console-layer)
   - [Mo.ProjectFilesToJson.Core (Core Library)](#moprojectfilestojsoncore-core-library)
8. [Tests](#tests)
   - [tests/Mo.ProjectFilesToJson.Test (Test Project)](#testsmo-projectfilestojsontest-test-project)
9. [Contributing](#contributing)
10. [License](#license)

---

## Why This Project?

Modern AI language models (LLMs) such as ChatGPT are incredibly powerful for:
- Generating documentation.
- Reviewing code snippets.
- Assisting with code refactoring.
- Enhancing code with additional features (e.g., optimizing performance, improving security, or integrating new functionalities). 

However, managing a large, evolving codebase when working with these models can be cumbersome. Many models (and their interfaces) don’t provide a straightforward way to keep multiple files in sync—some don’t even allow uploading a “project” for collective analysis.

Every time you update your code, you often have to re-upload or recreate those files in a new session, which quickly becomes tedious. This project bridges that gap by letting you selectively scan and consolidate all relevant files into a single output, ensuring you always have the latest version of your code ready to feed into an LLM.

Essentially, **Mo.ProjectFilesToJson** allows large repositories to be broken down or combined into a more LLM-friendly format.

---

## Project Highlights

- **Scan Any Project**: Currently configured for `.NET`, `Java`, `Node`, `Python`, and `VueJs`, but easily extensible to other languages or frameworks (e.g., Angular, React).
- **Smart Filtering**: Honors `.gitignore` patterns and custom include/exclude patterns to limit scanning to only relevant files (e.g., `.cs`, `.java`, `.py`, `.js`, `.json`).
- **Flexible Output**: Choose between:
  - **JSON** (machine-readable)
  - **Simple Divider** (human-readable)
- **User Settings**: The console app saves your last used configuration (source path, destination, format, etc.) so you can re-run quickly after code changes.
- **Scalable**: Works for small, medium, and large projects. You can also pick a subfolder to scan if you only need partial code.

---

## How It Works

1. **Prompt for or Load Existing Settings**  
   When you start the console app, it looks for a `userSettings.json` file. If found, it offers to reuse those settings (source directory, project type, etc.). Otherwise, it prompts you to enter new settings.

2. **Gather Files and Apply Filters**  
   The application reads the `.gitignore` (if available) for your chosen project type and also applies custom include/exclude patterns defined in `appsettings.json`.

3. **Read File Contents**  
   For each file that passes the filters, the application loads the file contents into memory.

4. **Format Output**  
   Depending on your choice (JSON or Simple Divider format), the tool transforms these files into a single textual output.

5. **Save the Results**  
   The resulting output is saved to a destination file of your choice. By default, it might be something like `C:\SomeFolder\Result.txt` if you leave certain inputs blank.

6. **Re-run as Needed**  
   Since your settings are saved, you can quickly re-run the tool to generate updated output whenever you modify your code.

---

## Installation & Setup

1. **Clone or Download** this repository.
2. Ensure you have [.NET 6 or higher](https://dotnet.microsoft.com/download) installed.
3. Open a terminal/command prompt in the project directory (where `Mo.ProjectFilesToJson.Console.csproj` is located).
4. **Build the solution** (if needed):
   ```bash
   dotnet build
   ```

---

## Usage

### Running the Console App

From within the solution directory, run:

```bash
cd Mo.ProjectFilesToJson.Console
dotnet run
```

The application will launch and:
- Attempt to load existing user settings from `userSettings.json`.
- If settings are found, it will prompt you:
  - **"Do you want to continue with these settings (Y/N)?"**
  - **Y** = Use the same settings and proceed directly to file scanning.
  - **N** = Prompt for new settings.
- If no settings file is found, it will prompt you for new settings.

### Selecting Projects and Paths

During the prompt, you will:
- Select your source project (e.g., DotNet, Java, Node, Python, VueJs).
- Enter the absolute source folder path (e.g., `D:\MyProjects\AwesomeApp`).
- Enter the destination path (file or folder path).
- Select the output text format:
  - **JSON**: The scanned files appear in a JSON array.
  - **Simple Divider**: Each file is wrapped with `--FILE <relative_path>` and `--END (<filename>)` markers.

### Formats: JSON or Simple Divider

#### JSON Format Example
```json
[
  {
    "FilePath": "Controllers/HomeController.cs",
    "Content": "public class HomeController {...}"
  },
  {
    "FilePath": "Views/Home/Index.cshtml",
    "Content": "<!-- HTML content -->"
  }
]
```

#### Simple Divider Format Example
```plaintext
--FILE Controllers/HomeController.cs
public class HomeController {
   ...
}
--END (HomeController.cs)

--FILE Views/Home/Index.cshtml
<!-- HTML content -->
--END (Index.cshtml)
```

---

## Configuration

Configuration settings are found in `appsettings.json`.

#### Adding More Projects
To add a new project type (e.g., Angular), update the `Projects` array:

```json
{
  "Name": "Angular",
  "OnlyIncludePatterns": ["*.ts", "*.json", "*.html", "*.css"],
  "AlsoExcludePatterns": [".git", "node_modules"]
}
```

---

## Architecture Overview

### Mo.ProjectFilesToJson.Console (Console Layer)
- `Program.cs`: Main entry point.
- `AppEngine.cs`: Orchestrates the workflow.
- `AppSetup.cs`: Handles dependency injection.

### Mo.ProjectFilesToJson.Core (Core Library)
- `GitIgnoreService.cs`: Handles `.gitignore` parsing.
- `FileScanService.cs`: Scans files based on filters.
- `FileFormatService.cs`: Formats output data.

---

## Tests

To run the tests:

```bash
cd tests
dotnet test
```

---

## Contributing

1. Fork the repository.
2. Create a feature branch.
3. Make your changes.
4. Submit a pull request.

---

## License

This project does not currently specify an open-source license. Check the repository for further details.