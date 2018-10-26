let addon = require('../native');

exports.handler = async (event) => {
    let url = event.pathParameters.url;
    console.log(`[URL] ${url} from ${event.headers['X-Forwarded-For']}`);

    console.time('Generate');
    let b64 = addon.str_to_base64_qr(decodeURIComponent(url));
    console.timeEnd('Generate');

    return {
        statusCode: 200,
        headers: { 'Content-Type': 'image/png' },
        body: b64,
        isBase64Encoded: true
    }
};