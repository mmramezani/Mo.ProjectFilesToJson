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
9. [Contributing](#contributing)
10. [License](#license)

---

## Why This Project?

Modern AI language models (LLMs) such as ChatGPT are incredibly powerful for:
- Generating documentation.
- Reviewing code snippets.
- Assisting with code refactoring.
- Enhancing code with additional features (e.g., optimizing performance or improving security).

However, managing a large, evolving codebase when working with these models can be cumbersome. Many models don’t provide an easy way to upload an entire project for collective analysis, forcing you to combine files manually or juggle them across multiple prompts.

**Mo.ProjectFilesToJson** lets you selectively scan your project, gather the relevant files (while respecting `.gitignore` and custom filters), and produce an easy-to-paste or upload **one-file** output suitable for LLM consumption.

---

## Project Highlights

- **Scan Any Project**: The default configuration supports `.NET`, `Java`, `Node`, `Python`, and `VueJs`, but you can easily add more frameworks (e.g., Angular, React).
- **Smart Filtering**:
  1. Automatically detects all `.gitignore` files **in your target folder** and applies their rules (including negative rules like `!somefile`).
  2. Uses custom include/exclude patterns defined in `appsettings.json`.
- **Flexible Output**: Choose between:
  - **JSON** (machine-readable)
  - **Simple Divider** (human-readable)
- **User Settings**: Caches your last run’s configuration in `userSettings.json`. Re-run quickly after making changes to your source code!
- **Scalable**: Works for small or large projects. You can also point to a subfolder if you only want part of a repository.

---

## How It Works

1. **Prompt for or Load Existing Settings**  
   - The console app loads `userSettings.json` if it exists. Otherwise, it prompts you for:
     - Which project type (e.g., `DotNet`, `Java`, etc.).
     - A source folder path.
     - A destination path.
     - Output format (JSON or Simple Divider).
2. **Gather Files and `.gitignore`**  
   - The app **recursively** scans the entire `SourceFolderPath`.
   - It **separates** out any `.gitignore` files it finds (including in subfolders).
   - It reads those `.gitignore` files and merges all their patterns.
3. **Apply Filters**  
   - First, `.gitignore` patterns are applied (removing files/folders they specify, unless “negated” with `!pattern`).
   - Then, custom include/exclude patterns from `appsettings.json` are applied (further filtering the final list).
4. **Read File Contents**  
   - The remaining files are read into memory.
5. **Format Output**  
   - Depending on your choice (JSON or Simple Divider), the tool transforms these files into a single textual output.
6. **Save the Results**  
   - The output is written to a destination file of your choice, such as `C:\Output\Result.txt`.
7. **Re-run as Needed**  
   - Because your settings are saved, you can simply re-run to generate fresh output anytime you modify your code.

---

## Installation & Setup

1. **Clone or Download** this repository.
2. Ensure you have [.NET 9+](https://dotnet.microsoft.com/download) installed.
3. Open a terminal or command prompt in the solution folder.
4. **Build the solution**:
   ```bash
   dotnet build
   ```

---

## Usage

### Running the Console App
From within the solution directory (where `Mo.ProjectFilesToJson.Console.csproj` is), run:

```bash
cd Mo.ProjectFilesToJson.Console
dotnet run
```

### Selecting Projects and Paths
You’ll be asked to:

- Choose one of the known project “profiles” (like DotNet, Java, Node, etc.).
- Specify the absolute source folder path (e.g., `D:\MyProjects\AwesomeApp`).
- Specify the destination file (where final output will be saved).
- Choose the text format (JSON or Simple Divider).

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

#### Simple Divider Example
```
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

### Adding More Projects
Open `appsettings.json` and add a new entry under "Projects":

```json
{
  "Name": "Angular",
  "OnlyIncludePatterns": ["*.ts", "*.html", "*.css", "*.json"],
  "AlsoExcludePatterns": ["node_modules", ".git"]
}
```

### Handling .gitignore Files
By default, Mo.ProjectFilesToJson automatically discovers all `.gitignore` files in your `SourceFolderPath` and applies their rules.

### Custom Include/Exclude Patterns
Each project entry in `appsettings.json` can specify:

- `OnlyIncludePatterns` (e.g., `['*.cs']` to include only C# files)
- `AlsoExcludePatterns` (e.g., `['.vs']` to exclude `.vs` folder)

---

## Architecture Overview

### Mo.ProjectFilesToJson.Console (Console Layer)
- `Program.cs`: Main entry point.
- `AppEngine.cs`: Orchestrates scanning/filtering/formatting workflows.
- `Helper.cs`: Utility methods.

### Mo.ProjectFilesToJson.Core (Core Library)
- `FileScanService.cs`: Scans folders.
- `GitIgnoreService.cs`: Reads `.gitignore` files.
- `CustomFilterService.cs`: Applies filtering.
- `FileFormatService.cs`: Formats output.

---

## Tests
Run tests via:
```bash
cd tests
dotnet test
```

---

## Contributing
Fork the repo, create a branch, make changes, and submit a PR.



