using CrudeEngine.CrudeEditor.Project;
using CrudeEngine.ECS;
using CrudeEngine.IO;
using ImGuiNET;
using System.Numerics;
using System.Reflection;
using System.Text.Json;

namespace CrudeEngine.CrudeEditor.Windows
{
    public class EntityEditorWindow : EditorWindow
    {
        private enum EditorState
        {
            SelectAction,
            CreateEntity,
            EditEntity,
            SaveEntity,
            LoadEntity
        }

        private EditorState _currentState = EditorState.SelectAction;
        private string _entityName = "NewEntity";
        private Entity? _currentEntity;

        private int _selectedComponentIndex = -1;
        private string _componentName = "";

        // File browser for save/load
        private string _fileBrowserPath = "";
        private string _selectedFilePath = "";

        public EntityEditorWindow() : base("Entity Editor")
        {
            DefaultSize = new Vector2(500, 600);
            RefreshComponentTypes();
        }

        private void RefreshComponentTypes()
        {
            
        }

        protected override void OnRender()
        {
            switch (_currentState)
            {
                case EditorState.SelectAction:
                    RenderActionSelection();
                    break;
                case EditorState.CreateEntity:
                    RenderCreateEntity();
                    break;
                case EditorState.EditEntity:
                    RenderEditEntity();
                    break;
                case EditorState.SaveEntity:
                    RenderSaveEntity();
                    break;
                case EditorState.LoadEntity:
                    RenderLoadEntity();
                    break;
            }
        }

        private void RenderActionSelection()
        {
            ImGui.TextColored(new Vector4(0.4f, 0.8f, 1.0f, 1.0f), "Entity Editor");
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.TextWrapped("Choose an action to get started:");
            ImGui.Spacing();

            if (ImGui.Button("Create New Entity", new Vector2(200, 40)))
            {
                _currentState = EditorState.CreateEntity;
            }

            ImGui.Spacing();

            if (ImGui.Button("Load Existing Entity", new Vector2(200, 40)))
            {
                var project = Project.Project.Current;
                _fileBrowserPath = project?.AssetsPath ?? AppContext.BaseDirectory;
                _currentState = EditorState.LoadEntity;
            }

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            if (ImGui.Button("Refresh Types"))
            {
                RefreshComponentTypes();
            }
            ImGui.SameLine();
        }

        private void RenderCreateEntity()
        {
            if (ImGui.Button("<- Back"))
            {
                _currentState = EditorState.SelectAction;
                return;
            }

            ImGui.Separator();
            ImGui.TextColored(new Vector4(0.4f, 0.8f, 1.0f, 1.0f), "Create New Entity");
            ImGui.Spacing();

            // Entity name input
            ImGui.Text("Entity Name:");
            ImGui.InputText("##EntityName", ref _entityName, 256);

            ImGui.Spacing();

            // Create button
            bool canCreate = !string.IsNullOrWhiteSpace(_entityName);
            if (!canCreate)
            {
                ImGui.BeginDisabled();
            }

            if (ImGui.Button("Create Entity", new Vector2(150, 30)))
            {
                CreateEntity();
            }

            if (!canCreate)
            {
                ImGui.EndDisabled();
            }
        }

        private void CreateEntity()
        {
            Entity e = new Entity();
            Game.Instance.GetCurrentScene().AddEntity(e);
            _currentEntity = e;
        }

        private void RenderEditEntity()
        {
            if (_currentEntity == null)
            {
                _currentState = EditorState.SelectAction;
                return;
            }

            // Top buttons
            if (ImGui.Button("<- Back"))
            {
                _currentEntity = null;
                _currentState = EditorState.SelectAction;
                return;
            }
            ImGui.SameLine();
            if (ImGui.Button("Save Entity"))
            {
                var project = Project.Project.Current;
                _fileBrowserPath = project?.AssetsPath ?? AppContext.BaseDirectory;
                _currentState = EditorState.SaveEntity;
            }

            ImGui.Separator();

            // Entity info header
            ImGui.TextColored(new Vector4(0.4f, 0.8f, 1.0f, 1.0f), $"Editing: {_entityName}");
            ImGui.Text($"Type: {_currentEntity.GetType().Name}");
            ImGui.Text($"ID: {_currentEntity.Id}");
            ImGui.Separator();

            // Transform section
            if (ImGui.CollapsingHeader("Transform", ImGuiTreeNodeFlags.DefaultOpen))
            {
                RenderTransformEditor();
            }

            // Components section
            if (ImGui.CollapsingHeader("Components", ImGuiTreeNodeFlags.DefaultOpen))
            {
                RenderComponentsEditor();
            }

            // Add component section
            if (ImGui.CollapsingHeader("Add Components"))
            {
                RenderAddComponent();
            }
        }

        private void RenderTransformEditor()
        {
            if (_currentEntity == null) return;

            var transform = _currentEntity.Transform;

            // Position
            var position = transform.Position;
            if (ImGui.DragFloat3("Position", ref position, 0.1f))
            {
                transform.Position = position;
            }

            // Rotation
            var rotation = transform.Rotation.AsVector4();
            if (ImGui.DragFloat4("Rotation", ref rotation, 1.0f))
            {
                transform.Rotation = new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
            }

            // Scale
            var scale = transform.Scale;
            if (ImGui.DragFloat3("Scale", ref scale, 0.01f))
            {
                transform.Scale = scale;
            }
        }

        private void RenderComponentsEditor()
        {
            if (_currentEntity == null) return;

            Dictionary<string, Component> components = new Dictionary<string, Component>();

            List<string> toRemove = [];

            foreach (var kvp in components)
            {
                ImGui.PushID(kvp.Key);

                if (ImGui.TreeNode($"{kvp.Key} ({kvp.Value.GetType().Name})"))
                {
                    RenderComponentProperties(kvp.Value);

                    ImGui.Spacing();
                    if (ImGui.Button("Remove"))
                    {
                        toRemove.Add(kvp.Key);
                    }

                    ImGui.TreePop();
                }

                ImGui.PopID();
            }

            // Remove marked components
            foreach (var name in toRemove)
            {
                _currentEntity.RemoveComponent(name);
            }
        }

        private void RenderComponentProperties(Component component)
        {
            var type = component.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var prop in properties)
            {
                RenderProperty(component, prop);
            }
        }

        private void RenderProperty(object obj, PropertyInfo prop)
        {
            var value = prop.GetValue(obj);
            var propType = prop.PropertyType;

            ImGui.PushID(prop.Name);

            if (propType == typeof(float))
            {
                float val = (float)(value ?? 0f);
                if (ImGui.DragFloat(prop.Name, ref val, 0.01f))
                {
                    prop.SetValue(obj, val);
                }
            }
            else if (propType == typeof(int))
            {
                int val = (int)(value ?? 0);
                if (ImGui.DragInt(prop.Name, ref val))
                {
                    prop.SetValue(obj, val);
                }
            }
            else if (propType == typeof(bool))
            {
                bool val = (bool)(value ?? false);
                if (ImGui.Checkbox(prop.Name, ref val))
                {
                    prop.SetValue(obj, val);
                }
            }
            else if (propType == typeof(Vector3))
            {
                var val = (Vector3)(value ?? Vector3.Zero);
                if (ImGui.DragFloat3(prop.Name, ref val, 0.01f))
                {
                    prop.SetValue(obj, val);
                }
            }
            else if (propType == typeof(Vector2))
            {
                var val = (Vector2)(value ?? Vector2.Zero);
                if (ImGui.DragFloat2(prop.Name, ref val, 0.01f))
                {
                    prop.SetValue(obj, val);
                }
            }
            else if (propType == typeof(string))
            {
                string val = (string)(value ?? "");
                if (ImGui.InputText(prop.Name, ref val, 512))
                {
                    prop.SetValue(obj, val);
                }
            }
            else
            {
                ImGui.Text($"{prop.Name}: {value?.ToString() ?? "null"}");
            }

            ImGui.PopID();
        }

        private void RenderAddComponent()
        {
            ImGui.Text("Component Name:");
            ImGui.InputText("##CompName", ref _componentName, 128);

            ImGui.Text("Select Component Type:");

            bool canAdd = _selectedComponentIndex >= 0 && !string.IsNullOrWhiteSpace(_componentName) && _currentEntity != null;
            if (!canAdd) ImGui.BeginDisabled();

            if (ImGui.Button("Add Component"))
            {
                try
                {
                    // TODO Add component
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to add component: {ex.Message}");
                }
            }

            if (!canAdd) ImGui.EndDisabled();
        }

        private void RenderSaveEntity()
        {
            if (ImGui.Button("<- Back to Editor"))
            {
                _currentState = EditorState.EditEntity;
                return;
            }

            ImGui.Separator();
            ImGui.TextColored(new Vector4(0.4f, 0.8f, 1.0f, 1.0f), "Save Entity");
            ImGui.Spacing();

            ImGui.Text("Entity Name:");
            ImGui.InputText("##SaveName", ref _entityName, 256);

            ImGui.Spacing();
            ImGui.Text($"Current Path: {_fileBrowserPath}");
            ImGui.Separator();

            // Navigation
            if (ImGui.Button(".."))
            {
                var parent = Directory.GetParent(_fileBrowserPath);
                if (parent != null)
                {
                    _fileBrowserPath = parent.FullName;
                }
            }

            // Directory listing
            if (ImGui.BeginChild("DirList", new Vector2(-1, 200), ImGuiChildFlags.Borders))
            {
                try
                {
                    var directories = Directory.GetDirectories(_fileBrowserPath);
                    foreach (var dir in directories)
                    {
                        var dirName = Path.GetFileName(dir);
                        if (ImGui.Selectable($"[DIR] {dirName}"))
                        {
                            _fileBrowserPath = dir;
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    ImGui.TextColored(new Vector4(1, 0, 0, 1), "Access denied");
                }
            }
            ImGui.EndChild();

            ImGui.Spacing();

            // Save button
            string savePath = Path.Combine(_fileBrowserPath, $"{_entityName}.json");
            ImGui.Text($"Will save to: {savePath}");

            if (ImGui.Button("Save Entity", new Vector2(150, 30)))
            {
                SaveCurrentEntity(savePath);
            }
        }

        private void SaveCurrentEntity(string fullPath)
        {
            if (_currentEntity == null) return;

            try
            {
                var project = Project.Project.Current;
                string relativePath = "";
                string fileName = Path.GetFileName(fullPath);
                string directory = Path.GetDirectoryName(fullPath) ?? "";

                if (project != null && directory.StartsWith(project.AssetsPath))
                {
                    relativePath = Path.GetRelativePath(project.AssetsPath, directory);
                    if (relativePath == ".") relativePath = "";
                }

                // Use EntityLoader to save
                EntityLoader.SaveEntityToAssets((dynamic)_currentEntity, fileName, relativePath);

                Console.WriteLine($"Entity saved to: {fullPath}");
                _currentState = EditorState.EditEntity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save entity: {ex.Message}");
            }
        }

        private void RenderLoadEntity()
        {
            if (ImGui.Button("<- Back"))
            {
                _currentState = EditorState.SelectAction;
                return;
            }

            ImGui.Separator();
            ImGui.TextColored(new Vector4(0.4f, 0.8f, 1.0f, 1.0f), "Load Entity");
            ImGui.Spacing();

            ImGui.Text($"Current Path: {_fileBrowserPath}");
            ImGui.Separator();

            // Navigation
            if (ImGui.Button(".."))
            {
                var parent = Directory.GetParent(_fileBrowserPath);
                if (parent != null)
                {
                    _fileBrowserPath = parent.FullName;
                }
            }

            // File browser
            if (ImGui.BeginChild("FileBrowser", new Vector2(-1, 300), ImGuiChildFlags.Borders))
            {
                try
                {
                    var directories = Directory.GetDirectories(_fileBrowserPath);
                    foreach (var dir in directories)
                    {
                        var dirName = Path.GetFileName(dir);
                        if (ImGui.Selectable($"[DIR] {dirName}"))
                        {
                            _fileBrowserPath = dir;
                        }
                    }

                    var files = Directory.GetFiles(_fileBrowserPath, "*.json");
                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileName(file);
                        bool isSelected = _selectedFilePath == file;
                        if (ImGui.Selectable(fileName, isSelected))
                        {
                            _selectedFilePath = file;
                            _entityName = Path.GetFileNameWithoutExtension(file);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    ImGui.TextColored(new Vector4(1, 0, 0, 1), "Access denied");
                }
            }
            ImGui.EndChild();

            ImGui.Spacing();

            if (!string.IsNullOrEmpty(_selectedFilePath))
            {
                ImGui.Text($"Selected: {Path.GetFileName(_selectedFilePath)}");

                if (ImGui.Button("Load Entity", new Vector2(150, 30)))
                {
                    LoadEntity(_selectedFilePath);
                }
            }
            else
            {
                ImGui.TextColored(new Vector4(0.6f, 0.6f, 0.6f, 1.0f), "Select a .json entity file to load");
            }
        }

        private void LoadEntity(string filePath)
        {
            try
            {
                string fullPath = Path.Combine(AssetLoader.ROOT_PATH, filePath);
                string jsonContent = File.ReadAllText(fullPath);

                // 1. Peek at the JSON to find the "TypeName"
                using JsonDocument doc = JsonDocument.Parse(jsonContent);
                string typeName = doc.RootElement.GetProperty("TypeName").GetString();
                Type entityType = Type.GetType(typeName);

                if (entityType == null) throw new Exception($"Type {typeName} not found.");

                // 2. Use Reflection to call SpawnEntity<ActualType>(filePath)
                var method = typeof(EntityLoader).GetMethod(nameof(EntityLoader.SpawnEntity));
                var genericMethod = method.MakeGenericMethod(entityType);

                // 3. Invoke and store the result
                _currentEntity = (Entity)genericMethod.Invoke(null, new object[] { filePath });

                // Update Editor State
                _entityName = Path.GetFileNameWithoutExtension(filePath);
                _currentState = EditorState.EditEntity;

                Console.WriteLine($"Loaded specific entity type: {entityType.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load entity: {ex.Message}");
            }
        }
    }
}