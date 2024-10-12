import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { FulltextsearchService } from "../../api";

export class FullTextSearchDataSource {

    #host: UmbControllerHost;

    constructor(host: UmbControllerHost) {
        this.#host = host;
    }

    async reindex(includeDescendants: boolean, nodeKey?: string) {
        return await tryExecuteAndNotify(this.#host, FulltextsearchService.postUmbracoFulltextsearchApiV5FulltextsearchReindexnodes({
            requestBody: {
                includeDescendants,
                nodeKey
            }
        }));
    }
}