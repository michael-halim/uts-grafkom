#version 330

out vec4 outputColor;
//in vec4 vertexColor; // unused if you don't use the 6 params
uniform vec3 objectColor; // pass the params

// this also works

// uniform vec4 ourColor;  //set in onrenderframe
void main(){
	//outputColor = vertexColor;
	outputColor = vec4(objectColor, 1);
}