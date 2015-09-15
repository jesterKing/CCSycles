/**
Copyright 2014 Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**/

#include "internal_types.h"

extern std::vector<ccl::DeviceInfo> devices;

unsigned int cycles_number_devices() {
	return (unsigned int)devices.size();
}

unsigned int cycles_number_cuda_devices() {
	int i = 0;
	for (ccl::DeviceInfo di : devices) {
		if (di.type == ccl::DeviceType::DEVICE_CUDA) i++;
	}

	return i;
}

const char *cycles_device_description(int i) {
	if (i>= 0 && i < devices.size())
		return devices[i].description.c_str();
	else
		return "-";
}

const char *cycles_device_id(int i) {
	if (i >= 0 && i < devices.size())
		return devices[i].id.c_str();
	else
		return "-";
}

int cycles_device_num(int i) {
	if (i >= 0 && i < devices.size())
		return devices[i].num;
	else
		return INT_MIN;
}

unsigned int cycles_device_advanced_shading(int i) {
	if (i >= 0 && i < devices.size())
		return devices[i].advanced_shading;
	else
		return 0;
}

unsigned int cycles_device_display_device(int i) {
	if (i >= 0 && i < devices.size())
		return devices[i].display_device;
	else
		return 0;
}

unsigned int cycles_device_pack_images(int i) {
	if (i >= 0 && i < devices.size())
		return devices[i].pack_images;
	else
		return 0;
}

unsigned int cycles_device_type(int i) {
	if (i >= 0 && i < devices.size())
		return devices[i].type;
	else
		return 0;
}


const char* cycles_device_capabilities() {
	static string capabilities = ccl::Device::device_capabilities();
	return capabilities.c_str();
}