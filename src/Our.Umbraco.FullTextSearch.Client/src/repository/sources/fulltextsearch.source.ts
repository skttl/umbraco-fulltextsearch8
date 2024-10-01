import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { FulltextsearchService } from "../../api";

export class FullTextSearchDataSource {

    #host: UmbControllerHost;

    constructor(host: UmbControllerHost) {
        this.#host = host;
    }

    async config() {
        return await tryExecuteAndNotify(this.#host, FulltextsearchService.getUmbracoFulltextsearchApiV5FulltextsearchConfig());
    }

    async indexStatus() {
        return await tryExecuteAndNotify(this.#host, FulltextsearchService.getUmbracoFulltextsearchApiV5FulltextsearchIndexstatus());
    }

    async incorrectIndexedNodes(pageNumber?: number) {
        return await tryExecuteAndNotify(this.#host, FulltextsearchService.getUmbracoFulltextsearchApiV5FulltextsearchIncorrectindexednodes({
            pageNumber
        }));
    }

    async indexedNodes(pageNumber?: number) {
        return await tryExecuteAndNotify(this.#host, FulltextsearchService.getUmbracoFulltextsearchApiV5FulltextsearchIndexednodes({
            pageNumber
        }));
    }

    async missingNodes(pageNumber?: number) {
        return await tryExecuteAndNotify(this.#host, FulltextsearchService.getUmbracoFulltextsearchApiV5FulltextsearchMissingnodes({
            pageNumber
        }));
    }

    async reindex(includeDescendants: boolean, nodeIds?: Array<(number)>) {
        return await tryExecuteAndNotify(this.#host, FulltextsearchService.postUmbracoFulltextsearchApiV5FulltextsearchReindexnodes({
            requestBody: {
                includeDescendants,
                nodeIds
            }
        }));
    }
}