#version 330 core  
out vec4 FragColor;  

struct Material {  
 sampler2D diffuse;  
 sampler2D normalMap;  
 sampler2D roughnessMap;  
 sampler2D displacementMap;  
 vec3 ambient;  
 vec3 diffuseColor;  
 vec3 specular;  
 float shininess;  
};  

in vec2 TexCoords;  
in vec3 FragPos;  
in vec3 Normal;  

uniform Material material;  
uniform vec3 lightPos;  
uniform vec3 viewPos;  

void main()  
{  
 // Displacement mapping  
 vec3 displacedPos = FragPos + texture(material.displacementMap, TexCoords).rgb * 0.1;  

 // Ambient lighting  
 vec3 ambient = material.ambient * texture(material.diffuse, TexCoords).rgb;  

 // Normal mapping  
 vec3 norm = normalize(texture(material.normalMap, TexCoords).rgb * 2.0 - 1.0);  

 // Diffuse lighting  
 vec3 lightDir = normalize(lightPos - displacedPos);  
 float diff = max(dot(norm, lightDir), 0.0);  
 vec3 diffuse = material.diffuseColor * diff * texture(material.diffuse, TexCoords).rgb;  

 // Specular lighting  
 vec3 viewDir = normalize(viewPos - displacedPos);  
 vec3 halfwayDir = normalize(lightDir + viewDir);  
 float spec = pow(max(dot(norm, halfwayDir), 0.0), material.shininess);  
 vec3 specular = material.specular * spec;  

 // Roughness adjustment  
 float roughness = texture(material.roughnessMap, TexCoords).r;  
 specular *= 1.0 - roughness;  

 // Combine results  
 vec3 result = ambient + diffuse + specular;  
 FragColor = vec4(result, 1.0);  
}
