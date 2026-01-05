import { manifests as entryPointManifests } from './entrypoints/manifest.ts';
import { manifests as entityActionManifests } from './actions/entity/manifest.ts';
import { manifests as modalManifests } from './modals/manifest.ts';
import { manifests as localizationManifests } from './localization/manifest.ts';

export const manifests: Array<UmbExtensionManifest> = [
    ...entryPointManifests,
    ...entityActionManifests,
    ...modalManifests,
    ...localizationManifests
];