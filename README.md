# AnalyzeThisAspNet-Auth0
Change the settings in the web.config file from x to your own application setting for Auth0 and SMTP email server:
<add key="auth0:ClientId" value="x" />
    <add key="auth0:ClientSecret" value="x" />
    <add key="auth0:CallbackURL" value="http://localhost:53562/LoginCallback.ashx" />
    <add key="auth0:Domain" value="x" />
    <add key="analyze:signingKey" value="x" />
    <add key="auth0:Connection" value="Username-Password-Authentication"/>
  </appSettings>
  <system.net>
    <mailSettings>
      <smtp from="myapp@auth0.com">
        <network userName="x" password="x" clientDomain="mailtrap.io" port="2525" host="mailtrap.io"/>
      </smtp>
    </mailSettings>
