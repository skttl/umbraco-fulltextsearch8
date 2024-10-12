import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { OpenAPI } from './../api/index.ts';
import { FullTextSearchDataSource } from "../repository/sources/fulltextsearch.source.ts";


export class FullTextSearchContext extends UmbControllerBase {

    #source: FullTextSearchDataSource;


    constructor(host: UmbControllerHost) {
        super(host);
        this.provideContext(FULLTEXTSEARCH_CONTEXT_TOKEN, this);
        this.#source = new FullTextSearchDataSource(host);

        this.consumeContext(UMB_AUTH_CONTEXT, (_auth) => {
            const umbOpenApi = _auth.getOpenApiConfiguration();
            OpenAPI.TOKEN = umbOpenApi.token;
            OpenAPI.BASE = umbOpenApi.base;
            OpenAPI.WITH_CREDENTIALS = umbOpenApi.withCredentials;
        });
    }

    async reindex(includeDescendants: boolean, nodeKey?: string) {
        await this.#source.reindex(includeDescendants, nodeKey);
    }
}

export default FullTextSearchContext;
export const FULLTEXTSEARCH_CONTEXT_TOKEN =
    new UmbContextToken<FullTextSearchContext>(FullTextSearchContext.name);