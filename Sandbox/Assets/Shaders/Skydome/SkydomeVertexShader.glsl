#version 330 core  

layout (location = 0) in vec3 aPos;  

uniform mat4 viewMatrix;  
uniform mat4 projectionMatrix;  
uniform vec3 cameraPosition;

void main()  
{  
   // Scale the vertex position  
   vec3 scaledPos = aPos * 900f;
   // Translate the vertex position to the camera's position
   scaledPos += cameraPosition;

   // Apply the view and projection transformations
   gl_Position = projectionMatrix * viewMatrix * vec4(scaledPos, 1.0);
}