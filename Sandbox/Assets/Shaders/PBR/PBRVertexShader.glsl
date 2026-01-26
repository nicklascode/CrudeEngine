#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;

uniform mat4 uMVP;

out vec2 TexCoords;
out vec3 FragPos;
out vec3 Normal;

uniform sampler2D displacementMap;
float strength = 0.001; // Displacement strength

void main()
{

    float height = texture(displacementMap, aTexCoord).r - 0.5;
    vec3 displacedPosition = aPos + normalize(aNormal) * (height * strength);

    gl_Position = uMVP * vec4(displacedPosition, 1.0);
    TexCoords = aTexCoord;
    FragPos = vec3(uMVP * vec4(displacedPosition, 1.0));
    Normal = mat3(transpose(inverse(uMVP))) * aNormal;
    Normal = normalize(Normal);
}
