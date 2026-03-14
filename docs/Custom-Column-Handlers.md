# Implementing Custom Explorer Columns via Property Handlers

## The Goal

To add a new, custom column to the Windows Explorer "Details" view that can display custom metadata for files (e.g., "File Hash", "Image Dimensions", "Document Word Count").

## The Modern Approach: The Windows Property System

Since Windows Vista, the legacy `IColumnProvider` interface has been deprecated. The correct and supported method for adding custom columns is to implement a **Property Handler**.

A property handler is a COM component (a DLL) that teaches Explorer how to read (and optionally write) custom metadata properties for a specific file type. Instead of telling Explorer "here is the text for my column," you tell Explorer "here is the value for the `MyProject.MyProperty` property," and Explorer handles the rest.

## Core Implementation Steps

Creating a property handler is a complex, multi-step process that requires deep integration with the Windows Shell. C++ is the recommended language, as implementing this in C# is exceptionally difficult due to the intricacies of COM interop.

Here is a high-level overview of the process:

### 1. Define Your Custom Property

First, you must define the property you want to display. This involves:
- Creating an XML schema file (e.g., `MyProperties.propdesc`).
- In this file, you define your property's name (e.g., `System.MyCompany.MyProperty`), its data type (string, number, date), how it should be displayed, and other attributes.
- This schema must be registered with the system using the `psreg.exe` tool or by calling the `PSRegisterPropertySchema` function.

### 2. Implement the Property Handler COM Server

You must create a DLL that contains a COM class implementing the following interfaces:
- **`IPropertyStore`**: This is the main interface. Explorer calls its `GetValue` method to request the value of your property for a given file. If you support writing, you must also implement `SetValue` and `Commit`.
- **`IInitializeWithStream` or `IInitializeWithFile`**: This interface is used by the shell to give your handler a stream or path to the file in question. Your `Initialize` method is where you would open the file, read its contents, and extract the data needed for your property.

### 3. Register the Handler

Once the DLL is built, it must be registered:
- The DLL must be registered as a COM server using `regsvr32.exe`.
- Specific registry keys must be created under `HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\PropertySystem\PropertyHandlers`. These keys associate your property handler (via its CLSID) with the file extension(s) it supports (e.g., `.txt`, `.docx`).

## The C# Challenge

While theoretically possible, creating a property handler in C# is not recommended because:
- **COM Interop Complexity**: You must manually define the native COM interfaces (`IPropertyStore`, etc.) in C# with perfect accuracy, including GUIDs, method signatures, and memory management rules (`PreserveSig`). This is very error-prone.
- **Registration**: Registering a C# assembly for COM interop in a way that the shell can robustly use is more complex than registering a native C++ DLL.
- **Performance and Stability**: Shell extensions are loaded directly into the Explorer process. Native code is generally considered safer and more performant for this low-level task.

## Path Forward

Due to the complexity, this feature will be planned as a C++ or C# project separate from the main scripting efforts. The `src` directory is reserved for this future development. The initial focus of this project will remain on features achievable through scripting and registry configuration.
