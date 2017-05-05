
/**
 * Service31032CallbackHandler.java
 *
 * This file was auto-generated from WSDL
 * by the Apache Axis2 version: 1.4.1  Built on : Aug 19, 2008 (10:13:39 LKT)
 */

    package com.ump.wcf;

    /**
     *  Service31032CallbackHandler Callback class, Users can extend this class and implement
     *  their own receiveResult and receiveError methods.
     */
    public abstract class Service31032CallbackHandler{



    protected Object clientData;

    /**
    * User can pass in any object that needs to be accessed once the NonBlocking
    * Web service call is finished and appropriate method of this CallBack is called.
    * @param clientData Object mechanism by which the user can pass in user data
    * that will be avilable at the time this callback is called.
    */
    public Service31032CallbackHandler(Object clientData){
        this.clientData = clientData;
    }

    /**
    * Please use this constructor if you don't want to set any clientData
    */
    public Service31032CallbackHandler(){
        this.clientData = null;
    }

    /**
     * Get the client data
     */

     public Object getClientData() {
        return clientData;
     }

        
           /**
            * auto generated Axis2 call back method for GetUserOperation method
            * override this method for handling normal response from GetUserOperation operation
            */
           public void receiveResultGetUserOperation(
                    com.ump.wcf.Service31032Stub.GetUserOperationResponse result
                        ) {
           }

          /**
           * auto generated Axis2 Error handler
           * override this method for handling error response from GetUserOperation operation
           */
            public void receiveErrorGetUserOperation(java.lang.Exception e) {
            }
                
           /**
            * auto generated Axis2 call back method for GetUserControlOrg method
            * override this method for handling normal response from GetUserControlOrg operation
            */
           public void receiveResultGetUserControlOrg(
                    com.ump.wcf.Service31032Stub.GetUserControlOrgResponse result
                        ) {
           }

          /**
           * auto generated Axis2 Error handler
           * override this method for handling error response from GetUserControlOrg operation
           */
            public void receiveErrorGetUserControlOrg(java.lang.Exception e) {
            }
                
           /**
            * auto generated Axis2 call back method for GetDataUsingDataContract method
            * override this method for handling normal response from GetDataUsingDataContract operation
            */
           public void receiveResultGetDataUsingDataContract(
                    com.ump.wcf.Service31032Stub.GetDataUsingDataContractResponse result
                        ) {
           }

          /**
           * auto generated Axis2 Error handler
           * override this method for handling error response from GetDataUsingDataContract operation
           */
            public void receiveErrorGetDataUsingDataContract(java.lang.Exception e) {
            }
                
           /**
            * auto generated Axis2 call back method for GetUserControlAgentOrExtension method
            * override this method for handling normal response from GetUserControlAgentOrExtension operation
            */
           public void receiveResultGetUserControlAgentOrExtension(
                    com.ump.wcf.Service31032Stub.GetUserControlAgentOrExtensionResponse result
                        ) {
           }

          /**
           * auto generated Axis2 Error handler
           * override this method for handling error response from GetUserControlAgentOrExtension operation
           */
            public void receiveErrorGetUserControlAgentOrExtension(java.lang.Exception e) {
            }
                
           /**
            * auto generated Axis2 call back method for GetData method
            * override this method for handling normal response from GetData operation
            */
           public void receiveResultGetData(
                    com.ump.wcf.Service31032Stub.GetDataResponse result
                        ) {
           }

          /**
           * auto generated Axis2 Error handler
           * override this method for handling error response from GetData operation
           */
            public void receiveErrorGetData(java.lang.Exception e) {
            }
                


    }
    