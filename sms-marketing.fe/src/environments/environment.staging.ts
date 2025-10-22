import { IEnvironment } from "@/shared/models/environment.models";

export const environment: IEnvironment = {
    production: false,
    baseUrl: 'https://smsapidev.huce.edu.vn',
    authGrantType: 'password',
    authClientId: 'client-web',
    authClientSecret: 'mBSQUHmZ4be5bQYfhwS7hjJZ2zFOCU2e',
    authScope: 'openid offline_access',
    appUrl: 'https://smsdev.huce.edu.vn',
};
