import { SitecorePageProps } from 'lib/page-props';
import { debug, getComponentLibraryStylesheetLinks } from '@sitecore-jss/sitecore-jss-nextjs';
import { Plugin } from '..';
import config from 'temp/config';

class ComponentThemesPlugin implements Plugin {
  order = 2;

  async exec(props: SitecorePageProps) {
    const startTime = Date.now();
    // Collect FEAAS, BYOC, SXA component themes
    props.headLinks.push(
      ...getComponentLibraryStylesheetLinks(
        props.layoutData,
        config.sitecoreEdgeContextId,
        config.sitecoreEdgeUrl
      )
    );

    debug.common(
      'finished getting Feaas theme data in %dms; name: %s',
      Date.now() - startTime,
      props?.layoutData?.sitecore?.route?.name
    );
    return props;
  }
}

export const componentThemesPlugin = new ComponentThemesPlugin();
