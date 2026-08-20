# StorySpoiler Testing with Selenium

This repository contains automated browser tests for the StorySpoiler application using .NET, NUnit, and Selenium WebDriver.

The test suite exercises the main user flows of the app, including login, invalid form validation, creating a spoiler, editing a spoiler, deleting a spoiler, and handling non-existent story IDs.

## Project overview

- Framework: .NET 8
- Test framework: NUnit
- Browser automation: Selenium WebDriver
- Browser: Google Chrome
- Driver management: Selenium ChromeDriver package

## Application under test

The tests target the StorySpoiler application running at:

- http://144.91.123.158:100

The login flow uses the default test account:

- Username: eli
- Password: eli123

## Included test scenarios

The suite includes the following automated checks:

1. Create story spoiler with invalid data
2. Create a random valid spoiler
3. Edit the last created spoiler title
4. Delete the last created spoiler
5. Attempt to edit a non-existent story spoiler
6. Attempt to delete a non-existent story spoiler

## Repository structure

- `StorySpoiler/StorySpoiler.slnx` - solution file
- `StorySpoiler/StorySpoiler/StorySpoiler.csproj` - project configuration and NuGet dependencies
- `StorySpoiler/StorySpoiler/StorySpoilerUnitTest1.cs` - Selenium NUnit tests

## Prerequisites

Before running the tests, make sure you have:

- .NET 8 SDK installed
- Google Chrome installed on the machine
- Access to the StorySpoiler application at the configured URL

## Run the tests

From the repository root:

```bash
dotnet test StorySpoiler/StorySpoiler/StorySpoiler.csproj
```

Or from the solution folder:

```bash
cd StorySpoiler
dotnet test
```

## Notes

- The tests launch a Chrome browser instance automatically.
- They maximize the browser window and use implicit waits for synchronization.
- If the app shows a browser alert during testing, the suite accepts it automatically where needed.
- The project is intended for UI regression and acceptance testing of the StorySpoiler web application.
