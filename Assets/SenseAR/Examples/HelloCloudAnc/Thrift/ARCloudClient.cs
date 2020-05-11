using System;
using Thrift;
using Thrift.Protocol;
using Thrift.Transport;
using UnityEngine;

public class ARCloudClient
{
    private const string AR_CLOUD_ADDR = "39.106.71.203";
    private const int AR_CLOUD_PORT = 80;

    public static RetInt createRoom()
    {
        TTransport transport = null;
        RetInt result = null;
        
        try
        {
            transport = new TSocket(AR_CLOUD_ADDR, AR_CLOUD_PORT);
            transport.Open();
            TProtocol protocol = new TBinaryProtocol(transport);
            ARCloud.Client client = new ARCloud.Client(protocol);

            result = client.createRoom();
        }
        catch (TException e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            if (transport != null)
            {
                transport.Close();
            }
        }
        
        return result;
    }

    public static RetStr enterRoom(int roomId)
    {

        TTransport transport = null;
        RetStr result = null;
        try
        {
            transport = new TSocket(AR_CLOUD_ADDR, AR_CLOUD_PORT);
            transport.Open();
            TProtocol protocol = new TBinaryProtocol(transport);
            ARCloud.Client client = new ARCloud.Client(protocol);
            result = client.getAnchor(roomId);
        }
        catch (TTransportException e)
        {
            Debug.LogError(e.Message);
            return null;
        }
        finally
        {
            if (transport != null)
            {
                transport.Close();
            }

        }
        return result;
    }

    public static RetStr saveAnchorId(int roomId, String anchorId)
    {
        TTransport transport = null;
        RetStr result = null;
        try
        {
            transport = new TSocket(AR_CLOUD_ADDR, AR_CLOUD_PORT);
            transport.Open();
            TProtocol protocol = new TBinaryProtocol(transport);
            ARCloud.Client client = new ARCloud.Client(protocol);
            result = client.saveAnchor(roomId, anchorId);
        }
        catch (TTransportException e)
        {
            Debug.LogError(e.Message);
            return null;
        }
        finally
        {
            if(transport != null)
            {
                transport.Close();
            }
            
        }
        return result;
    }
}
