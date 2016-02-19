/**
Copyright 2014-2015 Robert McNeel and Associates

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

#include "util_path.h"
#include "ccycles.h"

extern void _cleanup_scenes();
extern void _cleanup_sessions();
extern void _cleanup_shaders();
extern void _init_shaders();

std::ostream& operator<<(std::ostream& out, shadernode_type const &snt) {
	out << (int)snt;
	return out;
}

/* Hold the device information found on the system after initialisation. */
std::vector<ccl::DeviceInfo> devices;

/* Hold the logger function that potentially gets registered by a client. */
LOGGER_FUNC_CB logger_func = nullptr;

/* Need to initialise only once :) */
bool initialised{ false };
Logger logger;

std::vector<LOGGER_FUNC_CB> loggers;

void _cleanup_loggers()
{
	for (int i = 0; i < loggers.size(); i++) {
		loggers[i] = nullptr;
	}

	loggers.clear();
}

void cycles_path_init(const char* path, const char* user_path)
{
	ccl::path_init(string(path), string(user_path));
}

void cycles_initialise()
{
	if (!initialised) {
		devices = ccl::Device::available_devices();
		_init_shaders();
	}
}

void cycles_shutdown()
{
	if (!initialised) {
		return;
	}

	logger_func = nullptr;

	_cleanup_loggers();

	_cleanup_scenes();
	_cleanup_sessions();
	_cleanup_shaders();
}

void cycles_log_to_stdout(int tostdout)
{
	logger.tostdout = tostdout == 1;
}

void cycles_set_logger(unsigned int client_id, LOGGER_FUNC_CB logger_func_)
{
	loggers[client_id] = logger_func_;
}

unsigned int cycles_new_client()
{
	unsigned int logfunc_count{ 0 };
	for(auto logfunc : loggers) {
		if (logfunc == nullptr)
		{
			return logfunc_count;
		}
		++logfunc_count;
	}
	loggers.push_back(nullptr);
	assert(loggers.size() == logfunc_count+1);
	return logfunc_count;
}

void cycles_release_client(unsigned int client_id)
{
	loggers[client_id] = nullptr;
}

void cycles_f4_add(ccl::float4 a, ccl::float4 b, ccl::float4& res) {
	ccl::float4 r = a + b;
	res.x = r.x;
	res.y = r.y;
	res.z = r.z;
	res.w = r.w;
}

void cycles_f4_sub(ccl::float4 a, ccl::float4 b, ccl::float4& res) {
	ccl::float4 r = a - b;
	res.x = r.x;
	res.y = r.y;
	res.z = r.z;
	res.w = r.w;
}

void cycles_f4_mul(ccl::float4 a, ccl::float4 b, ccl::float4& res) {
	ccl::float4 r = a * b;
	res.x = r.x;
	res.y = r.y;
	res.z = r.z;
	res.w = r.w;
}

void cycles_f4_div(ccl::float4 a, ccl::float4 b, ccl::float4& res) {
	ccl::float4 r = a / b;
	res.x = r.x;
	res.y = r.y;
	res.z = r.z;
	res.w = r.w;
}

void cycles_tfm_inverse(const ccl::Transform& t, ccl::Transform& res) {
	ccl::Transform r = ccl::transform_inverse(t);
	res.x.x = r.x.x;
	res.x.y = r.x.y;
	res.x.z = r.x.z;
	res.x.w = r.x.w;

	res.y.x = r.y.x;
	res.y.y = r.y.y;
	res.y.z = r.y.z;
	res.y.w = r.y.w;

	res.z.x = r.z.x;
	res.z.y = r.z.y;
	res.z.z = r.z.z;
	res.z.w = r.z.w;

	res.w.x = r.w.x;
	res.w.y = r.w.y;
	res.w.z = r.w.z;
	res.w.w = r.w.w;
}
