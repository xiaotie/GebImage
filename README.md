GebImage
========

Geb.Image 是一款为图像分析目的而构建的易用、高性能的 C# 图像库。图像、视频这样的数据占人类数据总量的绝大部分，自 2008 年转型图像和视频开发时起，我就在找寻一个开发语言，能够高效的处理图像和视频数据，同时又具有高开发速度。经过多方比对，最终选择了 C#。C# 是一个成熟的快速开发语言，而打开 unsafe 后，可以直接用指针操作内存数据，能够实现接近于 C 语言的性能。Geb.Image 是我的一个尝试。

本项目有以下特色：

- 高性能：计算密集部分使用指针开发

- 自包含：是纯的 .net 库，不包含第三方 native dll

- 6.* 支持 .net 6 版本，不考虑对之前版本的支持

- 兼容 dotnet/CoreRT，方便编译为独立 exe 程序发布 

TODO LIST:

- [ ] 添加单元测试
- [ ] 添加示例代码
- [ ] 在 Geb.Image 中移除对 System.Drawing.Common 的依赖
- [ ] 添加 Geb.Image.Analysis 项目，这是一个不依赖于第三方库的图像分析库，推荐使用 Geb.Image.Analysis 库，当不够用时，再使用 OpenCV
- [ ] 添加 Geb.Image.Skia 项目，可通过 Skia 图形库，增强 Geb.Image 的图像处理能力
- [ ] 添加 Geb.Image.OpenCV 项目，可通过 OpenCV 图形库，增强图像分析能力
- [ ] 目前图像在内存中均是以 continuous 方式存储，后续将增加对非 continuous 方式的支持
