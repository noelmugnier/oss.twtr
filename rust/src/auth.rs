use actix_web::{web, Responder, post, Result};

pub fn config(cfg: &mut web::ServiceConfig) {
    cfg.service(
        web::scope("api/account")
            .service(register)
            .service(login)
    );
}

#[post("/register")]
async fn register(user_info:web::Json<RegisterUser>) -> Result<impl Responder> {
    let connection = &mut establish_connection();
    let user_id = register_user(&user_info.into_inner(), connection)?;
    Ok(web::Json(user_id))
}

#[post("/login")]
async fn login(user_credentials:web::Json<LoginUser>) -> Result<impl Responder> {
    let is_authenticated = login_user(&user_credentials.into_inner()).unwrap_or(false);
    Ok(web::Json(is_authenticated))
}