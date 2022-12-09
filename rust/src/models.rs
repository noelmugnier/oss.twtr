use diesel::prelude::*;

#[derive(Queryable)]
pub struct Account {
    pub id: i32,
    pub username: String,
    pub password: String,
    pub salt: String,
}