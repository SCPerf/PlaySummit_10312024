import { SitecorePageProps } from 'lib/page-props';
import { GetServerSidePropsContext, GetStaticPropsContext } from 'next';
import { debug, getSiteRewriteData } from '@sitecore-jss/sitecore-jss-nextjs';
import { Plugin } from '..';
import { siteResolver } from 'lib/site-resolver';
import config from 'temp/config';

class SitePlugin implements Plugin {
  order = 0;

  async exec(props: SitecorePageProps, context: GetServerSidePropsContext | GetStaticPropsContext) {
    if (context.preview) return props;
    const startTime = Date.now();

    const path =
      context.params === undefined
        ? '/'
        : Array.isArray(context.params.path)
        ? context.params.path.join('/')
        : context.params.path ?? '/';

    // Get site name (from path)
    const siteData = getSiteRewriteData(path, config.sitecoreSiteName);

    // Resolve site by name
    props.site = siteResolver.getByName(siteData.siteName);

    debug.common(
      'finished getting site data in %dms; site: %s',
      Date.now() - startTime,
      props.site.name
    );

    return props;
  }
}

export const sitePlugin = new SitePlugin();
