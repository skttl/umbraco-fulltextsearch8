import { UmbEntryPointOnInit } from '@umbraco-cms/backoffice/extension-api';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';
import { manifests as entityActionManifests } from './actions/entity/manifest.ts';
import { manifests as modalManifests } from './modals/manifest.ts';

export const onInit: UmbEntryPointOnInit = (_host, extensionRegistry) => {

    // register them here. 
    extensionRegistry.registerMany([
        ...entityActionManifests,
        ...modalManifests
    ]);

    _host.consumeContext(UMB_AUTH_CONTEXT, (_auth) => {
        // const umbOpenApi = _auth.getOpenApiConfiguration();
        // OpenAPI.TOKEN = umbOpenApi.token;
        // OpenAPI.BASE = umbOpenApi.base;
        // OpenAPI.WITH_CREDENTIALS = umbOpenApi.withCredentials;
    });

};