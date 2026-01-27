using CrudeEngine.IO;

namespace CrudeEngine.CrudeEditor.Project
{
    public class Project
    {
        public static Project? Current { get; private set; }

        public string Name { get; private set; }
        public string ProjectPath { get; private set; }
        public string AssetsPath => Path.Combine(ProjectPath, "Assets");

        private Project(string name, string projectPath)
        {
            Name = name;
            ProjectPath = projectPath;
        }

        public static Project Load(string projectPath)
        {
            if (!Directory.Exists(projectPath))
                throw new DirectoryNotFoundException($"Project path not found: {projectPath}");

            string name = Path.GetFileName(projectPath);
            var project = new Project(name, projectPath);

            Current = project;
            ApplyAssetPath(project);

            Console.WriteLine($"Loaded project: {name} at {projectPath}");
            return project;
        }

        public static void Unload()
        {
            Current = null;
            AssetLoader.SetRootPath(string.Empty);
        }

        private static void ApplyAssetPath(Project project)
        {
            if (Directory.Exists(project.AssetsPath))
            {
                AssetLoader.SetRootPath(project.AssetsPath);
            }
            else
            {
                Console.WriteLine($"Warning: Assets folder not found at {project.AssetsPath}");
            }
        }
    }
}