import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { FullTextSearchOptions, IndexedNodeResult, IndexStatus, OpenAPI } from './../api/index.ts';
import { FullTextSearchDataSource } from "../repository/sources/fulltextsearch.source.ts";
import { UmbObjectState, UmbStringState } from "@umbraco-cms/backoffice/observable-api";


export class FullTextSearchContext extends UmbControllerBase {

    #source: FullTextSearchDataSource;

    #config = new UmbObjectState<FullTextSearchOptions | undefined>(undefined);
    public readonly config = this.#config.asObservable();

    #indexStatus = new UmbObjectState<IndexStatus | undefined>(undefined);
    public readonly indexStatus = this.#indexStatus.asObservable();

    #indexedNodes = new UmbObjectState<IndexedNodeResult | undefined>(undefined);
    public readonly indexedNodes = this.#indexedNodes.asObservable();

    #incorrectIndexedNodes = new UmbStringState<IndexedNodeResult | undefined>(undefined);
    public readonly incorrectIndexedNodes = this.#incorrectIndexedNodes.asObservable();

    #missingIndexedNodes = new UmbStringState<IndexedNodeResult | undefined>(undefined);
    public readonly missingIndexedNodes = this.#missingIndexedNodes.asObservable();


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

    async getConfig() {
        const { data } = await this.#source.config();

        if (data) {
            this.#config.setValue(data);
        }
    }

    async reindex(includeDescendants: boolean, nodeIds?: Array<(number)>) {
        await this.#source.reindex(includeDescendants, nodeIds);
    }

    async getIndexStatus() {
        const { data } = await this.#source.indexStatus();

        if (data) {
            this.#indexStatus.setValue(data);
        }
    }

    async getIndexedNodes(pageNumber?: number) {
        const { data } = await this.#source.indexedNodes(pageNumber);
        if (data) {
            this.#indexedNodes.setValue(data);
        }
    }

    async getIncorrectIndexedNodes(pageNumber?: number) {
        const { data } = await this.#source.incorrectIndexedNodes(pageNumber);
        if (data) {
            this.#incorrectIndexedNodes.setValue(data);
        }
    }

    async getMissingNodes(pageNumber?: number) {
        const { data } = await this.#source.missingNodes(pageNumber);
        if (data) {
            this.#missingIndexedNodes.setValue(data);
        }
    }
}

export default FullTextSearchContext;
export const FULLTEXTSEARCH_CONTEXT_TOKEN =
    new UmbContextToken<FullTextSearchContext>(FullTextSearchContext.name);