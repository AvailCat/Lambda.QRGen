# What's this
It's a AWS Lambda function that generate url or string qrcode via AWS APIGateway

# Compile
Current it's not possible to compile native node addon on Lambda, So you need compile it manually and upload zip file in control dashboard.

You can't compile it locally because the tool used to make node binding does not support static link, compile and run on different system with different library version.

- Follow [Lambda Execution Environment](https://docs.aws.amazon.com/lambda/latest/dg/current-supported-versions.html) to findout which version system was used to run Lambda function, create a new EC2 instance run that version system.
- Install same node version as your Lambda config, tested on 8.10
- Install rust nightly toolchain and `yun groupinstall "Development Tools"`
- Compile by run `npm install`
- Once done, you can delete `node_modules` and `native/target` directory to reduce size or you will not able to upload it to AWS Lambda,
- The archive size should be around 2MB, upload and configure AWS APIGateway to start use.