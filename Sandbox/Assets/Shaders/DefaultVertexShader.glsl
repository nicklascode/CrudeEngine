#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

#define MAX_LIGHTS 8

struct Light {
    vec3 position;
    vec3 color;
};

uniform Light uLights[MAX_LIGHTS];
uniform int lightCount;

out vec2 TexCoords;
out vec3 Lighting; // pass to fragment
out vec3 FragTexColor; // we’ll use this in frag shader if needed

vec3 CalculatePointLight(Light light, vec3 fragPos, vec3 normal)
{
    vec3 lightDir = normalize(light.position - fragPos);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 diffuse = diff * light.color;
    // Attenuation
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (1.0 + 0.09 * distance + 0.032 * (distance * distance));
    
    return diffuse * attenuation;
}

void main()
{
    mat4 modelView = uView * uModel;
    gl_Position = uProjection * modelView * vec4(aPos, 1.0);

    vec3 worldPos = vec3(uModel * vec4(aPos, 1.0));
    vec3 worldNormal = mat3(transpose(inverse(uModel))) * aNormal;

    vec3 lighting = vec3(0.1); // ambient
    for (int i = 0; i < lightCount; ++i) {
        lighting += CalculatePointLight(uLights[i], worldPos, worldNormal);
    }

    TexCoords = aTexCoord;
    Lighting = lighting;
}
