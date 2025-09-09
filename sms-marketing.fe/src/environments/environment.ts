import { IEnvironment } from "@/shared/models/environment.models";

export const environment: IEnvironment = {
    production: true,
    baseUrl: 'http://localhost:5069/',
    authGrantType: 'password',
    authClientId: 'client-web2',
    authClientSecret: 'mBSQUHmZ4be5bQYfhwS7hjJZ2zFOCU2e',
    authScope: 'openid offline_access',
    appUrl: 'http://localhost:4200',
};
