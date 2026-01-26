#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;

uniform mat4 uMVP;
uniform float uTime;
uniform float uWaveAmplitude;
uniform float uWaveFrequency;

out vec2 TexCoords;
out vec3 Normal;
out vec3 Position;


// Wave height function for normal approximation
float getWave(vec3 pos) {
    // Use a combination of sine waves for more complex wave patterns
       float wave = uWaveAmplitude * sin(uWaveFrequency * pos.x + uTime) +
                    uWaveAmplitude * sin(uWaveFrequency * pos.z + uTime) +
                    uWaveAmplitude * sin(uWaveFrequency * (pos.x + pos.z) + uTime);
       return wave;
}


void main()
{
    vec3 pos = aPos;
    float wave = getWave(pos);
    vec3 displacedPos = pos + vec3(0.0, wave, 0.0);

    // Approximate tangent and bitangent for normal
    float offset = 0.05;

    vec3 dx = vec3(offset, getWave(pos + vec3(offset, 0.0, 0.0)) - wave, 0.0);
    vec3 dz = vec3(0.0, getWave(pos + vec3(0.0, 0.0, offset)) - wave, offset);
    vec3 dynamicNormal = normalize(cross(dz, dx));

    // Final outputs
    gl_Position = uMVP * vec4(displacedPos, 1.0);
    TexCoords = aTexCoord;
    Normal = dynamicNormal;
    Position = displacedPos;
}
