<p align="center">
  <img width="300" src=".github/logo.gif"/>
</p>

**A Blazor WASM implementation of a Things3 inspired todo list showcasing Supabase C# features.**

A [live version of this project](https://todo.acupofjose.com) is hosted through Github Pages.

## Current Project State

- [x] Authentication using 3rd Party Providers (Github)
- [x] Realtime, Responsive Models
- [ ] (In Progress) Offline Support
  - [ ] Store application state offline (cross platform)
  - [ ] Sync application state after returning online
- [x] Responsive Routing
- [x] Keyboard Shortcuts
- [ ] Gesture Support
- [ ] Drag-and-drop Support
- [x] Light Mode / Dark Mode
- [ ] Todos
  - [x] Create, Update, Trash, and Delete Todos
  - [ ] Views for different Todo States
    - [x] Inbox
    - [ ] Today
    - [ ] Upcoming
    - [x] Trash
  - [x] Set a Due Time / Due At
  - [ ] Add/Remove todo from a list
  - [ ] Add/Remove labels from a todo
  - [ ] Reorder Todos using Drag and drop

## Integration with Supabase

Most of the code, where possible, has been placed into the [SupasharpTodo.Shared project](/SupasharpTodo.Shared/) and leverages Dependency Injection to handle cross-platform implementations.

The interfaces are specified in [SupasharpTodo.Shared.Interfaces](/SupasharpTodo.Shared/Interfaces/).

What will likely interest most developers using this repo as an example of Supabase's features are the files found in:

- [ServiceCollectionExtensions.cs](/SupasharpTodo.Shared/Extensions/ServiceCollectionExtensions.cs) - handles registering Supabase in the App Builder.
- [SupabaseAuthenticationStateProvider.cs](/SupasharpTodo.Shared/Providers/SupabaseAuthenticationStateProvider.cs) - registers with Blazor's built-in authentication
- [SupabaseSessionProvider.cs](/SupasharpTodo.Shared/Providers/SupabaseSessionProvider.cs) - handles storing the session locally (platform dependent)

## For Developers

This project is built using Tailwind as the CSS framework, which requires compiling on a css change. The fastest way to develop is within the `SupasharpTodo.BlazorWASM` project.

You'll need to do an `npm install` in the root of the project to install Tailwind and its dependencies.

Two commands need to be running simultaneously for development:

```bash
# Either using `concurrently`
npm run dev:wasm

# Or in separate terminals...
# In root of repo
npm run dev:tailwind:wasm

# In `SupasharpTodo.BlazorWASM`
dotnet watch
```
