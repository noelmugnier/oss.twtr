import http from 'k6/http';
import { check } from 'k6';
import exec from 'k6/execution';

export let options = {
  vus: 500,
  duration:'5s'  
}

export default function () {
    const date = Date.now();
    const url = 'https://localhost:7120/api/auth/register';
    const payload = JSON.stringify({
        username: 'superUser_' + date + '_' + exec.vu.idInTest,
        password: 'superTestPassword',
        confirmPassword:'superTestPassword'
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    let res = http.post(url, payload, params);
    check(res, {
        'register is status 200': (r) => r.status === 200,
    });
    
    const loginurl = 'https://localhost:7120/api/auth/login';
    const payloadloginurl = JSON.stringify({
        username: 'superUser_' + date + '_' + exec.vu.idInTest,
        password: 'superTestPassword',
    });

    let resLogin = http.post(loginurl, payloadloginurl, params);
    check(resLogin, {
        'login is status 200': (r) => r.status === 200,
    });

    let bodyloginurl = JSON.parse(resLogin.body);
    const paramsloginurl = {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + bodyloginurl.token
        },
    };

    const urlTweet = 'https://localhost:7120/api/tweets';

    let count = 0;
    while(count < 10) {
        const payloadTweet = JSON.stringify({
            message: count + ': super test for user: ' + 'superUser_' + date + '_' + exec.vu.idInTest,
        });
        
        let resTweet = http.post(urlTweet, payloadTweet, paramsloginurl);

        check(resTweet, {
            'post 10 new tweets is status 201': (r) => r.status === 201,
        });
        
        count++;
    }

}