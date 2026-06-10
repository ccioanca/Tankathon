shader_type canvas_item;

uniform vec4 highlight_color : source_color = vec4(1.0, 1.0, 1.0, 0.6);
uniform vec4 shadow_color : source_color = vec4(0.0, 0.0, 0.0, 0.6);
uniform float highlight_size : hint_range(0.0, 20.0) = 4.0;
uniform float shadow_size : hint_range(0.0, 20.0) = 4.0;
uniform vec2 offset = vec2(-1.0, -1.0); // Direction of light
uniform int blur_samples : hint_range(3, 15) = 6; // Smoothness steps

void fragment() {
	vec4 original = texture(TEXTURE, UV);
	vec2 ps = TEXTURE_PIXEL_SIZE;
	vec2 light_dir = normalize(offset);
	
	float highlight_alpha_sum = 0.0;
	float shadow_alpha_sum = 0.0;
	
	// Accumulate alpha samples along the light direction vector
	for (int i = 1; i <= blur_samples; i++) {
		float progress = float(i) / float(blur_samples);
		
		// Push samples further out based on size and loop progress
		vec2 hi_offset = light_dir * (highlight_size * progress) * ps;
		vec2 sh_offset = light_dir * (shadow_size * progress) * ps;
		
		highlight_alpha_sum += texture(TEXTURE, UV + hi_offset).a;
		shadow_alpha_sum += texture(TEXTURE, UV - sh_offset).a;
	}
	
	// Average out the sampled alpha values
	float highlight_avg = highlight_alpha_sum / float(blur_samples);
	float shadow_avg = shadow_alpha_sum / float(blur_samples);
	
	// Calculate smooth inner edges based on the missing alpha values
	float inner_highlight = original.a * (1.0 - highlight_avg);
	float inner_shadow = original.a * (1.0 - shadow_avg);
	
	// Blend the soft colors over the original texture
	vec3 final_color = mix(original.rgb, shadow_color.rgb, inner_shadow * shadow_color.a);
	final_color = mix(final_color, highlight_color.rgb, inner_highlight * highlight_color.a);
	
	COLOR = vec4(final_color, original.a);
}
