shader_type spatial;

uniform vec3 CLR : source_color = vec3(255,255,255);

void vertex() {
	// Called for every vertex the material is visible on.
}

float rand(vec2 co){
    return fract(sin(dot(co, vec2(12.9898, 78.233))) * (43758.5453));
}

void fragment() {
	ALBEDO = CLR;
	if(UV.x <= 0.05 || UV.x >= 0.95){
		ALBEDO = vec3(0.0,0.0,0.0);
	}
	if(UV.y <= 0.05 || UV.y >= 0.95){
		ALBEDO = vec3(0.0,0.0,0.0);
	}

}
//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
