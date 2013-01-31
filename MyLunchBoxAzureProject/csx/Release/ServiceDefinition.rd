<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="MyLunchBoxAzureProject" generation="1" functional="0" release="0" Id="7cd7ffdc-a666-4d16-ab15-50938a4332bf" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="MyLunchBoxAzureProjectGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="MyLunchBox:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/LB:MyLunchBox:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/LB:MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" defaultValue="">
          <maps>
            <mapMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MapCertificate|MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </maps>
        </aCS>
        <aCS name="MyLunchBox:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MapMyLunchBox:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="">
          <maps>
            <mapMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MapMyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </maps>
        </aCS>
        <aCS name="MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="">
          <maps>
            <mapMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MapMyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </maps>
        </aCS>
        <aCS name="MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="">
          <maps>
            <mapMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MapMyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </maps>
        </aCS>
        <aCS name="MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MapMyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </maps>
        </aCS>
        <aCS name="MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MapMyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </maps>
        </aCS>
        <aCS name="MyLunchBox:MyLunchBox" defaultValue="">
          <maps>
            <mapMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MapMyLunchBox:MyLunchBox" />
          </maps>
        </aCS>
        <aCS name="MyLunchBox:MyLunchBoxDevelopmentEntities" defaultValue="">
          <maps>
            <mapMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MapMyLunchBox:MyLunchBoxDevelopmentEntities" />
          </maps>
        </aCS>
        <aCS name="MyLunchBoxInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MapMyLunchBoxInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:MyLunchBox:Endpoint1">
          <toPorts>
            <inPortMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBox/Endpoint1" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput">
          <toPorts>
            <inPortMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBox/Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </toPorts>
        </lBChannel>
        <sFSwitchChannel name="SW:MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp">
          <toPorts>
            <inPortMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBox/Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
          </toPorts>
        </sFSwitchChannel>
      </channels>
      <maps>
        <map name="MapCertificate|MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" kind="Identity">
          <certificate>
            <certificateMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBox/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </certificate>
        </map>
        <map name="MapMyLunchBox:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBox/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapMyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" kind="Identity">
          <setting>
            <aCSMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBox/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </setting>
        </map>
        <map name="MapMyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" kind="Identity">
          <setting>
            <aCSMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBox/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </setting>
        </map>
        <map name="MapMyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" kind="Identity">
          <setting>
            <aCSMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBox/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </setting>
        </map>
        <map name="MapMyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBox/Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </setting>
        </map>
        <map name="MapMyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBox/Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </setting>
        </map>
        <map name="MapMyLunchBox:MyLunchBox" kind="Identity">
          <setting>
            <aCSMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBox/MyLunchBox" />
          </setting>
        </map>
        <map name="MapMyLunchBox:MyLunchBoxDevelopmentEntities" kind="Identity">
          <setting>
            <aCSMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBox/MyLunchBoxDevelopmentEntities" />
          </setting>
        </map>
        <map name="MapMyLunchBoxInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBoxInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="MyLunchBox" generation="1" functional="0" release="0" software="C:\Users\yaqiZhao\Documents\Visual Studio 2010\Projects\MyLunchBox\MyLunchBoxAzureProject\csx\Release\roles\MyLunchBox" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp" />
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp" portRanges="3389" />
              <outPort name="MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/SW:MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="" />
              <aCS name="MyLunchBox" defaultValue="" />
              <aCS name="MyLunchBoxDevelopmentEntities" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;MyLunchBox&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;MyLunchBox&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBox/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBoxInstances" />
            <sCSPolicyUpdateDomainMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBoxUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBoxFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="MyLunchBoxUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="MyLunchBoxFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="MyLunchBoxInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="0440a375-2cd0-4e4c-9394-793ec62af162" ref="Microsoft.RedDog.Contract\ServiceContract\MyLunchBoxAzureProjectContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="4de4603d-8132-43d2-9cc4-94961b095d90" ref="Microsoft.RedDog.Contract\Interface\MyLunchBox:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBox:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="e125c0ed-c481-483b-85b3-98cb7729782e" ref="Microsoft.RedDog.Contract\Interface\MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/MyLunchBoxAzureProject/MyLunchBoxAzureProjectGroup/MyLunchBox:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>