using CrudeEngine.ECS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace CrudeEngine.IO
{
    public class EntityLoader
    {
        public static T SpawnEntity<T>(string entityPath) where T : Entity
        {
            string fullPath = Path.Combine(AssetLoader.ROOT_PATH, entityPath);
            
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Entity file not found: {fullPath}");
            }

            string jsonContent = File.ReadAllText(fullPath);
            
            // First, deserialize to get the wrapper with type information
            using JsonDocument document = JsonDocument.Parse(jsonContent);
            JsonElement root = document.RootElement;
            
            // Extract the type name from the wrapper
            string? typeName = root.GetProperty("TypeName").GetString();
            if (string.IsNullOrEmpty(typeName))
            {
                throw new InvalidOperationException("Entity file missing TypeName property");
            }
            
            // Get the actual Type from the assembly-qualified name
            Type? entityType = Type.GetType(typeName);
            if (entityType == null)
            {
                throw new TypeLoadException($"Could not load type: {typeName}");
            }
            
            // Verify the loaded type is compatible with the requested type
            if (!typeof(T).IsAssignableFrom(entityType))
            {
                throw new InvalidCastException($"Entity type {entityType.Name} is not assignable to {typeof(T).Name}");
            }
            
            // Deserialize the Entity property using the correct type
            JsonElement entityElement = root.GetProperty("Entity");
            T? newEntity = JsonSerializer.Deserialize(entityElement.GetRawText(), entityType) as T;

            if (newEntity != null)
            {
                Game.Instance.GetCurrentScene().AddEntity(newEntity);
                Console.WriteLine($"Entity of type {entityType.Name} has been loaded and spawned in");
                return newEntity;
            }
            else
            {
                throw new InvalidOperationException($"Could not deserialize entity of type {entityType.Name}");
            }
        }

        public static void SaveEntityToAssets<T>(T entity, string entityName, string entityPath) where T : Entity
        {
            var fullPath = Path.Combine(AssetLoader.ROOT_PATH, entityPath, entityName);

            var wrapper = new
            {
                TypeName = typeof(T).AssemblyQualifiedName,
                Entity = entity
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(wrapper, options);
            File.WriteAllText(fullPath, json);

            Console.WriteLine("Entity has been exported to assets");
        }
    }
}
