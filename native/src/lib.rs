#![feature(duration_as_u128)]
#[macro_use]

extern crate neon;
extern crate image;
extern crate base64;
extern crate qrcode;

use neon::prelude::*;

fn get_qr_base64(string: &str) -> String {
    let qr = qrcode::QrCode::new(string).unwrap();

    let image = qr
        .render::<image::Luma<u8>>()
        .min_dimensions(256, 256)
        .max_dimensions(256, 256)
        .build();

    let iw = image.width();
    let ih = image.height();
    let mut buf = Vec::with_capacity((iw * ih) as usize);;
    image::png::PNGEncoder::new(&mut buf).encode(&image.into_raw(), iw, ih, image::Gray(8)).unwrap();

    return base64::encode(&buf);
}

fn str_to_base64_qr(mut cx: FunctionContext) -> JsResult<JsString> {
    let url = cx.argument::<JsString>(0)?.value();
    
    let b64 = get_qr_base64(&url);

    Ok(cx.string(b64))
}

register_module!(mut cx, {
    cx.export_function("str_to_base64_qr", str_to_base64_qr)
});

#[cfg(test)]
mod tests {
    use std::time::SystemTime;
    use super::get_qr_base64;

    #[test]
    fn it_works() {
        let start_time = SystemTime::now();
        let b64 = get_qr_base64("https://example.org");
        println!("Generated {}kb data in {}ms", b64.len() / 1024, start_time.elapsed().unwrap().as_millis());
        assert_eq!(&b64[..6], "iVBORw")
    }
}