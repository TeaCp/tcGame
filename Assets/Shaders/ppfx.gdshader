shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;
uniform int pixelSize: hint_range(2, 16, 2) = 6;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec2 iRes = 1.0 / SCREEN_PIXEL_SIZE;
    
    float onePixelSizeX = 1.0 / iRes.x;
    float onePixelSizeY = 1.0 / iRes.y;
    
    float cellSizeX =  float(pixelSize) * onePixelSizeX;
    float cellSizeY = float(pixelSize) * onePixelSizeY; 
    
    float x = cellSizeX * floor(UV.x / cellSizeX);
    float y = cellSizeY * floor(UV.y / cellSizeY);
    
    //COLOR = textureLod(screen_texture, vec2(x, y), 0.0);
	COLOR = textureLod(screen_texture, SCREEN_UV, 0.0);
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
