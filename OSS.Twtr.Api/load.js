import http from 'k6/http';
import { check } from 'k6';

export let options = {
  vus: 100,
  duration:'5s'  
}

export default function () {
    const url = 'https://localhost:7120/api/register';
    const payload = JSON.stringify({
        username: 'superUser',
        password: 'superTestPassword',
        confirmPassword:'superTestPassword'
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    let res = http.post(url, payload, params);
    let body = JSON.parse(res.body);
    const urlTweet = 'https://localhost:7120/api/tweet';
    const payloadTweet = JSON.stringify({
        userId: body.id,
        message: 'super test',
    });

    let count = 0;
    while(count < 10) {
        let resTweet = http.post(urlTweet, payloadTweet, params);

        check(resTweet, {
            'is status 200': (r) => r.status === 200,
        });
        
        count++;
    }

}