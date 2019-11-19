# MarySue Encoder

![](https://github.com/atonasting/marysue-encoder/workflows/Build/badge.svg)

把任意文字和玛丽苏体的姓名进行加密转换。

例：

原文：
> 苟利国家生死以
> 岂因祸福避趋之
> ————林则徐

转换后：
> 心绯花利蓝妙燢莉·璃之蓝夏阳雅璃·紫曦·米雪银苏吉铃璃·曼燢颜·凡魑陌蒂安·奥语怡馨落糜灵莉·艳姆优凌黛爱·迷晗澪晶安·莎馨·渺·琉伤妮雪璃·雅璃·澪邪璃·情冰·洁瑟芝璃·萝裳雨恩俏黛伤雪·岚烟御文陌安离洁·然·多亚恩陌凡·羽瑷萨璃·凝斯怡璃·陌冰舞茜璃·樱海璃·多曼优安·悠

注：每次生成的密文是随机的，但都能解出相同的原文。

## Demo

https://funnyjs.com/marysue/

## Run

Written in C#, running on dotnet core platform.

1. Visit https://www.microsoft.com/net/core to install .net core runtime
1. git clone code
1. cd marysue-encoder
1. dotnet restore
1. dotnet run
1. Visit http://localhost:5001

## Develop

基本算法

1. 首先准备一串字符作为文本素材，并指定某个字符作为分隔符；
1. 使用随机Key将原文进行AES加密，将密钥附在密文前；
1. 将加密后的文本随机切分成长度1~8的byte数组，将数组转换成ulong数字；
1. 设文本素材数量为n，则将ulong数字转换成n进制数，以文本素材作为此数的具体数字；
1. 使用分隔符将切分后的字符串连接起来，即得密文；
1. 解密时反向计算即可。

给开发者的参考：

- 修改appsettings.json中的素材与分隔符，可以制作自己版本的密钥生成器；
- 可以附加前后缀文本作为格式与加密版本判断。

## Improvment

- 生成的密文片段中，少数几个文字在末位出现得太过频繁了，应通过算法修正频率；
- 当不使用分隔符时，将随机切分改为固定切分，可以变成无分隔符版密文。
