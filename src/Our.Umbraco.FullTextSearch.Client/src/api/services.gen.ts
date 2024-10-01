// This file is auto-generated by @hey-api/openapi-ts


import type { CancelablePromise } from './core/CancelablePromise';
import { OpenAPI } from './core/OpenAPI';
import { request as __request } from './core/request';
import type { $OpenApiTs } from './types.gen';

export class FulltextsearchService {
    /**
 * @returns unknown OK
 * @throws ApiError
 */
    public static getUmbracoFulltextsearchApiV5FulltextsearchConfig(): CancelablePromise<$OpenApiTs['/umbraco/fulltextsearch/api/v5/fulltextsearch/config']['get']['res'][200]> {
        return __request(OpenAPI, {
    method: 'GET',
    url: '/umbraco/fulltextsearch/api/v5/fulltextsearch/config'
});
    }
    
    /**
 * @returns unknown OK
 * @throws ApiError
 */
    public static getUmbracoFulltextsearchApiV5FulltextsearchIncorrectindexednodes(data: $OpenApiTs['/umbraco/fulltextsearch/api/v5/fulltextsearch/incorrectindexednodes']['get']['req'] = {}): CancelablePromise<$OpenApiTs['/umbraco/fulltextsearch/api/v5/fulltextsearch/incorrectindexednodes']['get']['res'][200]> {
        const { pageNumber } = data;
        return __request(OpenAPI, {
    method: 'GET',
    url: '/umbraco/fulltextsearch/api/v5/fulltextsearch/incorrectindexednodes',
    query: {
        pageNumber
    }
});
    }
    
    /**
 * @returns unknown OK
 * @throws ApiError
 */
    public static getUmbracoFulltextsearchApiV5FulltextsearchIndexednodes(data: $OpenApiTs['/umbraco/fulltextsearch/api/v5/fulltextsearch/indexednodes']['get']['req'] = {}): CancelablePromise<$OpenApiTs['/umbraco/fulltextsearch/api/v5/fulltextsearch/indexednodes']['get']['res'][200]> {
        const { pageNumber } = data;
        return __request(OpenAPI, {
    method: 'GET',
    url: '/umbraco/fulltextsearch/api/v5/fulltextsearch/indexednodes',
    query: {
        pageNumber
    }
});
    }
    
    /**
 * @returns unknown OK
 * @throws ApiError
 */
    public static getUmbracoFulltextsearchApiV5FulltextsearchIndexstatus(): CancelablePromise<$OpenApiTs['/umbraco/fulltextsearch/api/v5/fulltextsearch/indexstatus']['get']['res'][200]> {
        return __request(OpenAPI, {
    method: 'GET',
    url: '/umbraco/fulltextsearch/api/v5/fulltextsearch/indexstatus'
});
    }
    
    /**
 * @returns unknown OK
 * @throws ApiError
 */
    public static getUmbracoFulltextsearchApiV5FulltextsearchMissingnodes(data: $OpenApiTs['/umbraco/fulltextsearch/api/v5/fulltextsearch/missingnodes']['get']['req'] = {}): CancelablePromise<$OpenApiTs['/umbraco/fulltextsearch/api/v5/fulltextsearch/missingnodes']['get']['res'][200]> {
        const { pageNumber } = data;
        return __request(OpenAPI, {
    method: 'GET',
    url: '/umbraco/fulltextsearch/api/v5/fulltextsearch/missingnodes',
    query: {
        pageNumber
    }
});
    }
    
    /**
 * @returns unknown OK
 * @throws ApiError
 */
    public static postUmbracoFulltextsearchApiV5FulltextsearchReindexnodes(data: $OpenApiTs['/umbraco/fulltextsearch/api/v5/fulltextsearch/reindexnodes']['post']['req'] = {}): CancelablePromise<$OpenApiTs['/umbraco/fulltextsearch/api/v5/fulltextsearch/reindexnodes']['post']['res'][200]> {
        const { requestBody } = data;
        return __request(OpenAPI, {
    method: 'POST',
    url: '/umbraco/fulltextsearch/api/v5/fulltextsearch/reindexnodes',
    body: requestBody,
    mediaType: 'application/json'
});
    }
    
}