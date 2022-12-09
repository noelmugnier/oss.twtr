pub mod user {

use diesel::PgConnection;
use serde::{Deserialize, Serialize};

#[derive(Deserialize, Serialize)]
pub struct RegisterUser {
    username: String,
    password: String,
    confirm: String,
}

pub fn handle(user_info: &RegisterUser, connection: &PgConnection) -> Result<String, &'static str> {
    if user_info.username.len() < 3 {
        return Err("Username must be at least 3 characters long");
    }
    
    if user_info.password.len() < 5 {
        return Err("Password must be at least 5 characters long");
    }

    if user_info.password != user_info.confirm {
        return Err("Passwords do not match");
    }

    let results = accounts
        .filter(published.eq(true))
        .limit(5)
        .load::<Account>(connection)
        .expect("Error loading posts");

    Ok(String::from("token"))
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn should_return_invalid_username() {
        let user_credentials = RegisterUser{
            username: String::from("ue"),
            password: String::new(),
            confirm: String::new(),
        };

        assert_eq!(handle(&user_credentials), Err("Username must be at least 3 characters long"));
    }
    
    #[test]
    fn should_return_invalid_password() {
        let user_credentials = RegisterUser{
            username: String::from("test"),
            password: String::from("inv"),
            confirm: String::new(),
        };

        assert_eq!(handle(&user_credentials), Err("Password must be at least 5 characters long"));
    }
    
    #[test]
    fn should_return_password_do_not_match() {
        let user_credentials = RegisterUser{
            username: String::from("test"),
            password: String::from("tester"),
            confirm: String::from("test1"),
        };

        assert_eq!(handle(&user_credentials), Err("Passwords do not match"));
    }
    
    #[test]
    fn should_return_token() {
        let user_credentials = RegisterUser{
            username: String::from("test"),
            password: String::from("tester"),
            confirm: String::from("tester"),
        };

        assert_eq!(handle(&user_credentials), Ok(String::from("token")));
    }
    
    #[test]
    fn should_insert_user_in_database(){
        let user_credentials = RegisterUser{
            username: String::from("test"),
            password: String::from("tester"),
            confirm: String::from("tester"),
        };
         
        handle(&user_credentials);
        let user = crate::db::get_user_by_username(&user_credentials.username);

        assert_eq!(user.is_some(), true);
    }    
}
}