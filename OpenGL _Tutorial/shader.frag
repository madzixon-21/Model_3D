#version 330 core
out vec4 FragColor;

uniform vec3 lightColor;
uniform vec3 lightPos;
uniform vec3 lightPos2;
uniform vec3 viewPos;

in vec3 Normal;
in vec3 FragPos;
in vec3 Color;

void main()
{
    float ambientStrength = 0.2;
    vec3 ambient = ambientStrength * lightColor;

    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    vec3 lightDir2 = normalize(lightPos2 - FragPos);

    float diff = max(dot(norm, lightDir), 0.0);
    float diff2 = max(dot(norm, lightDir2), 0.0);
    vec3 diffuse = diff * lightColor;
    vec3 diffuse2 = diff2 * lightColor;

    float specularStrength = 0.3;
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    vec3 reflectDir2 = reflect(-lightDir2, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32); 
    float spec2 = pow(max(dot(viewDir, reflectDir2), 0.0), 32);
    vec3 specular = specularStrength * spec * lightColor;
    vec3 specular2 = specularStrength * spec2 * lightColor;

    vec3 result = (ambient + diffuse + specular) * Color;
    vec3 result2 = (ambient + diffuse2 + specular2) * Color;
    
    FragColor = vec4(result+result2, 1.0);
}