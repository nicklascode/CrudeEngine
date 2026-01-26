#version 330 core  

out vec4 FragColor;  

uniform vec2 resolution; // Screen resolution  

void main()  
{  
	// Calculate the normalized coordinates of the fragment
	vec2 uv = gl_FragCoord.xy / resolution;
	// Create a gradient effect based on the vertical position
	float gradient = uv.y;
	// Set the color based on the gradient
	vec3 color = vec3(0.5, 0.7, 1.0) * gradient; // Sky blue gradient
	// Set the alpha to 1.0 for full opacity
	FragColor = vec4(color, 1.0);
}