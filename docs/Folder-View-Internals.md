# Understanding Folder View Settings Internals

## The Goal

One of the primary goals of this project is to allow users to save and apply specific view settings (like column layout, sorting, and view mode) to individual folders.

## The Challenge: `BagMRU` and `ItemIDList`

Windows Explorer stores these folder-specific settings in the current user's registry, primarily under two keys:
- `HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags`
- `HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\BagMRU`

The `Bags` key contains the actual view settings (the "what"), with each setting stored in a subkey named with a numerical ID (e.g., "345").

The `BagMRU` key maps a folder path to one of these Bag IDs (the "where"). This mapping is the critical and difficult part. The `BagMRU` key does not store a simple list of paths. Instead, it mirrors the shell namespace hierarchy (Desktop -> C: -> Users -> ...), and the folder paths are stored in a binary format known as an **`ItemIDList`** (or PIDL).

An `ItemIDList` is a low-level shell structure that uniquely identifies any object in the Windows shell, whether it's a file, a folder, a control panel applet, or a network location. This binary data is not human-readable and cannot be easily parsed with standard scripting languages.

## The Problem with a Pure Scripting Approach

To reliably save a view for a specific folder path (e.g., `C:\Users\Me\Documents`), a script would need to:

1.  Traverse the `BagMRU` registry structure, which is a tree of numerical keys.
2.  At each level, read the binary `ItemIDList` data.
3.  Parse this binary data to determine what folder it represents.
4.  Compare it with the target folder path.
5.  Once the correct `BagMRU` entry is found, identify its corresponding `Bag` ID.
6.  Extract the settings from the `Bags` key.

Steps 2 and 3 are extremely complex. The format of the `ItemIDList` is variable and requires deep integration with the Windows Shell API to parse correctly. Attempting to do this in PowerShell would be brittle and likely fail for many types of folders (network drives, libraries, special folders, etc.).

## The Proposed Solution: C# and P/Invoke

The most robust way to solve this problem is to use a compiled language like C# that can directly interact with the Windows Shell API.

The plan is to create a small C# helper application or library that:
1.  Takes a folder path as input.
2.  Uses the `SHGetIDListFromObject` or a similar Windows API function to get the `ItemIDList` for that path.
3.  Scans the registry to find a matching `ItemIDList` in the `BagMRU` structure.
4.  Returns the corresponding `Bag` ID.

This approach offloads the complex parsing to the Windows API itself, which is guaranteed to be accurate. This C# component can then be called from our main application or even from PowerShell to provide the necessary information to save, restore, or modify a view for a specific folder.
