# Unityでのビルドに関して

---

# TensowFlow LiteのFrameworkのビルドに関して

bazelは1.1.0を使用。
ただしカスタムオペレータを使わなくなったので以下未使用。

```
% git clone https://github.com/google/mediapipe
```

```
% git clone https://github.com/tensorflow/tensorflow
% cd ~/Work/tensorflow
% bazel build -c opt --cxxopt=--std=c++11 tensorflow/lite/c:libtensorflowlite_c.so
% cp ~/Work/tensorflow/bazel-bin/tensorflow/lite/c/libtensorflowlite_c.so ~/Work/Camera04/Assets/TensorFlowLite/Plugins/macOS/libtensorflowlite_c.bundle
```

```
% bazel build -c opt --cpu ios_arm64 --copt -Os --copt -DTFLITE_GPU_BINARY_RELEASE --copt -fvisibility=hidden --copt=-fembed-bitcode --linkopt -s --strip always --cxxopt=-std=c++14 //tensorflow/lite/delegates/gpu:tensorflow_lite_gpu_framework --apple_platform_type=ios
% cp ~/Work/tensorflow/bazel-bin/tensorflow/lite/delegates/gpu/tensorflow_lite_gpu_framework.zip ~/Work/Camera04/Assets/TensorFlowLite/Plugins/iOS/tensorflow_lite_gpu_framework.framework
```

---
