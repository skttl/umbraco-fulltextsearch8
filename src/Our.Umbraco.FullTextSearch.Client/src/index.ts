import { UmbEntryPointOnInit } from '@umbraco-cms/backoffice/extension-api';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';
import { manifests as entityActionManifests } from './actions/entity/manifest.ts';
import { manifests as modalManifests } from './modals/manifest.ts';
import { manifests as localizationManifests } from './localization/manifest.ts';
import { manifests as contextManifests } from './context/manifest.ts';
import { OpenAPI } from './api/index.ts';

export const onInit: UmbEntryPointOnInit = (_host, extensionRegistry) => {

    // register them here. 
    extensionRegistry.registerMany([
        ...entityActionManifests,
        ...modalManifests,
        ...localizationManifests,
        ...contextManifests
    ]);


    _host.consumeContext(UMB_AUTH_CONTEXT, (_auth) => {
        const umbOpenApi = _auth.getOpenApiConfiguration();
        OpenAPI.TOKEN = umbOpenApi.token;
        OpenAPI.BASE = umbOpenApi.base;
        OpenAPI.WITH_CREDENTIALS = umbOpenApi.withCredentials;
    });

};