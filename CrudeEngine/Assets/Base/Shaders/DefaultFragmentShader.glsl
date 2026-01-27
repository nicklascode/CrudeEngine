#version 330 core

in vec2 TexCoords;
in vec3 Lighting;

out vec4 FragColor;

struct Material {
    vec3 color;
    sampler2D texture;
};

uniform Material uMaterial;
uniform bool useTexture;

void main()
{
    vec3 baseColor = uMaterial.color;
    if (useTexture)
        baseColor *= texture(uMaterial.texture, TexCoords).rgb;

    // Lighting calculation
    float lightIntensity = max(dot(Lighting, vec3(0.0, 0.0, 1.0)), 0.0);
    for (int i = 0; i < 3; ++i) {
        baseColor[i] += lightIntensity;
    }
    FragColor = vec4(baseColor, 1.0);

}
