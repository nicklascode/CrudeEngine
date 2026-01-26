#version 330 core  
out vec4 FragColor;  

in vec2 TexCoords;  
in vec3 Normal;  
in vec3 Position;  

uniform float uTime;  
uniform vec3 uLightDir = normalize(vec3(-0.4, 1.0, 0.2));  
uniform sampler2D uTexture;  

// Water color settings  
vec3 waterColor = vec3(0.0, 0.5, 1.0);  

void main()  
{  
   // Calculate lighting intensity  
   float lightIntensity = max(dot(normalize(Normal), uLightDir), 0.0);  

   // Sample texture for additional detail  
   vec3 textureColor = texture(uTexture, TexCoords).rgb;  

   // Move the texture coordinates based on time for animation
   vec2 animatedTexCoords = TexCoords + vec2(uTime * 0.001, uTime * 0.001);
   textureColor = texture(uTexture, animatedTexCoords).rgb;

   // Combine base water color with texture and lighting  
   vec3 finalColor = mix(waterColor, textureColor, 0.5) * lightIntensity;  

   // Output final fragment color  
   FragColor = vec4(finalColor, 1.0);  
}
