# Project Roadmap: Windows Customizer

*This document outlines the detailed plan for each module of the Windows Customizer application.*

## 1. Bloatware Removal Module
*   **Task:** Develop a feature to identify and uninstall pre-installed Windows applications (e.g., Mixed Reality Portal, Solitaire Collection) and disable non-essential services and startup applications to improve system performance and cleanliness.
*   **Requirements:**
    *   Administrator privileges are mandatory for all operations in this module.
    *   The application must be able to query all installed `AppX` packages for the current user and system-wide.
    *   Implement functionality to execute the `Remove-AppxPackage` PowerShell command to uninstall selected applications.
    *   The application must list all system services (e.g., via `Get-Service`) and allow the user to change their startup type (e.g., to 'Disabled') via a command like `Set-Service`.
    *   The application must be able to identify and disable startup programs, likely by modifying keys in the Windows Registry (`HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`).
*   **Complexity:** **Medium**. The core functions are straightforward, but building a safe, reliable, and user-friendly interface around them requires careful implementation.

## 2. Essential Program Installation Module
*   **Task:** Create a user-friendly interface for selecting and installing a curated list of essential third-party programs.
*   **Requirements:**
    *   The recommended approach is to leverage the native Windows Package Manager (`winget`). This greatly simplifies the process of finding, downloading, and silently installing applications.
    *   The UI will act as a front-end for `winget`, generating and executing commands like `winget install <PackageName>`.
    *   The application will need to parse the output from the `winget` process to provide real-time feedback (downloading, installing, success, failure) to the user.
    *   A default, editable list of recommended applications (e.g., 7-Zip, Notepad++, VSCode) should be provided.
*   **Complexity:** **Medium**. While `winget` does the heavy lifting, creating a robust wrapper that handles process management and output parsing is a non-trivial task.

## 3. Windows Explorer Customization Module
*   **Task:** Provide a centralized panel of toggles and options to customize the behavior and appearance of Windows Explorer.
*   **Requirements:**
    *   This module will interact heavily with the Windows Registry, primarily under `HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer`.
    *   Must provide options for common tweaks, such as: showing hidden files, showing file extensions, hiding the "3D Objects" folder, and disabling the new context menus in Windows 11.
    *   A robust registry interaction service is a prerequisite for this module.
*   **Complexity:** **High**. The number of possible customizations is vast, and many require knowledge of specific, often undocumented, registry keys. Thorough testing is required to ensure changes don't corrupt the user's profile.

## 4. Custom .msc Views Module
*   **Task:** Allow a user to build a custom Microsoft Management Console (`.msc`) file by selecting from a list of available snap-ins.
*   **Requirements:**
    *   Research and understand the XML-based file format of `.msc` files.
    *   The application will need to programmatically generate a valid `.msc` XML file based on user selections.
    *   Provide a list of common and useful snap-ins (e.g., Event Viewer, Services, Disk Management, Group Policy Editor).
*   **Complexity:** **Medium**. The main challenge is correctly structuring the XML file according to the required schema, which must be determined.

## 5. Microsoft Defender Killswitch Module
*   **Task:** Implement a clear, simple, and temporary way to disable Microsoft Defender's real-time protection and the Windows Firewall.
*   **Requirements:**
    *   **Critical:** This feature must prominently display warnings about the security risks of being unprotected.
    *   Administrator privileges are mandatory.
    *   For Defender, the application will execute the `Set-MpPreference -DisableRealtimeMonitoring $true` PowerShell command. It must also provide a corresponding "Enable" button that runs the same command with `$false`.
    *   For the Firewall, the application can use `netsh advfirewall set allprofiles state off`. An "Enable" button must be present to run `state on`.
*   **Complexity:** **Low to Medium**. The commands are simple, but the UI/UX must be handled responsibly to ensure users understand the action they are taking.

## 6. Search Bar Customization Module
*   **Task:** Provide options to control the Windows Search feature, focusing on disabling integrated web search results.
*   **Requirements:**
    *   This will be achieved by modifying the `DisableSearchBoxSuggestions` DWORD value within the `HKEY_CURRENT_USER\SOFTWARE\Policies\Microsoft\Windows\Explorer` registry key.
    *   The UI will be a simple toggle switch.
*   **Complexity:** **Low**. This is a straightforward registry modification.

## 7. UI Customization Integration (Windhawk)
*   **Task:** Integrate with the third-party application "Windhawk" to allow users to browse and apply its advanced UI modifications.
*   **Requirements:**
    *   **Uncertainty:** This task is entirely dependent on Windhawk's capabilities for programmatic interaction. It may have a command-line interface (CLI), a developer API, or no integration points at all.
    *   If a CLI is available, this module would act as a front-end, executing commands to list and apply mods.
    *   If there is no integration method, this task may be reduced to simply providing a button to download and launch the Windhawk application.
*   **Complexity:** **Unknown/High**. Without the ability to research Windhawk, the implementation cannot be determined. It is a high-risk task.

## 8. Registry Key Management Module
*   **Task:** This is a foundational, non-UI module that will be used by many other features. It will provide a safe and reliable way to interact with the Windows Registry.
*   **Requirements:**
    *   Create a static C# class or service that encapsulates registry operations.
    *   It must use the `Microsoft.Win32.Registry` classes.
    *   Methods should include: `Read`, `Write`, `DeleteKey`, `KeyExists`, etc.
    *   The module must gracefully handle exceptions like `SecurityException` (access denied) and `KeyNotFoundException`.
*   **Complexity:** **Medium**. Building a truly robust and error-proof service is more involved than one-off registry calls.

## 9. Invasive Windows Service Management Module
*   **Task:** Provide tools to disable invasive or unwanted Windows features and services, such as telemetry and OneDrive integration.
*   **Requirements:**
    *   This module will combine techniques from other modules.
    *   Disabling services (e.g., `DiagTrack` for telemetry) will use the same method as the Bloatware module (`Set-Service`).
    *   Blocking telemetry servers can be done by appending entries to the `hosts` file located at `C:\Windows\System32\drivers\etc\hosts`. This requires careful, atomic file I/O with admin rights.
    *   Blocking can also be done by creating outbound firewall rules, using the `netsh` command.
*   **Complexity:** **High**. Modifying the `hosts` file is a high-risk operation that can impact network connectivity if done incorrectly. Combining multiple techniques into one cohesive feature adds to the complexity.


# Addendum


# IMPORTANT !!!!!!!!!!!!!!!!!!!!!!
# DO NOT TEST ANYTHING, DO NOT TEST ANY PART OF THE APP. ONCE YOU ARE DONE CODING, TELL ME AND DO A COMPLETE RE-READ OF THE CODE TO TRY TO CATCH OBVIOUS MISTAKES. I WILL HANDLE TESTING. 
#YOU CAN COMPILE/RUN THE APP FOR EVERYTHING THAT IS VISUAL/DESIGN

## 1.
*The application will be built in C# with WinUI 3. Make use of the glass/transparency effects and custom controls offered to design the app. Lets add all the app design to this section. Try to make a central file with a "skin" that's easily replaceable/customizable/reusable. 
*The windows programs will be listed with a checkbox for batch uninstalling. The start section should not reflect the one from the task manager, a lot of things are missing from it. We should see hidden apps and services that also start at boot.

## 2.
* This one should also support batch install with checkbox

## 3.
* I want to clarify that i would like the option to hide/show a singular folder or a singular file, or a list of files extension, etc. I would like to have customizable infoTip per file type. Ex: I choose Videos, check the attributes i want to see and the program modifies the registry like this: "InfoTip"="prop:System.Comment;System.DateCreated;System.Link.TargetParsingPath;System.Image.Dimensions;System.Size;System.Media.Duration". Try to hide the libraries from everywhere: sidebar, homepage, context-menu, the "Desktop" folder view. Add an interface to easily modify a file's icon (or by file types if possible). Find a way to add information to the status bar at the bottom of explorer.exe, especially adding the folder size even if we have to calculate it (add a caching mechanism, a delay before starting to calculate, etc. to prevent reading too much from the disk). I would backup the registry before almost every change. Try to find other customizable info, folder views, search views, search behavior, custom themes and skins, whatever you can think of and update me on this before implementing any of the new features you found.

## 4.
* This one is good, maybe add a drag n drop if its available.

## 5.
*This one will be a standalone app that runs in the system tray. Try to build this app like a template and in a way that is easy to reuse because i will need more kill switches in the future.
* It should disable ALL the modules from microsoft defender, all the real time protection, file scanning processes, etc. and optionally kill the internet connection as well (on by default). Make a separate github for it called "help-me-crack". Tell me if you are unable to create the repo but code the app in any case.

## 6.
* This should remove the web suggestions yes but also try preventing the weird behavior that happens when you type and erase the query (it changes the start menu view), try to see if we can manipulate the results to show more installed apps and local files instead of microsoft store apps.

## 7.
* I have added the source code of my installed add ons here : "C:\Users\Administrator\Desktop\Github Repos\Projects\windows-customizer\WindHawk"
* Basically the app would offer to download Windhawk and apply my custom config which is a ready-made glass/acrylic theme with other customization. For the code modification, users should use windhawk as it has a great interface already.

## 8.
* Again, don't forget the automatic backups before each action.

## 9.
*Some modules off the top of my head: OneDrive (requires registry modifications to really disable it), Windows Spotlight (beware: implemented in
*multiple spots), Everyhing related to telemetry (registry modif again) add this list to etc/host file : **https://github.com/hagezi/dns-blocklists/blob/main/hosts/native.winoffice.txt** and warn the user to also install it at the DNS level. Do some more research to see if there are other annoying or useless modules.

